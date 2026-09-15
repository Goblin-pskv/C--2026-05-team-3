using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.RegistrationCommands
{
    public class CreateRegistrationCommandHandle : IRequestHandler<CreateRegistrationCommand, Result>
    {

        private readonly IRegistrationRepository _registrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IValidator<CreateRegistrationCommand> _validator;
        private readonly ILogger<CreateRegistrationCommandHandle> _logger;

        public CreateRegistrationCommandHandle(
           IRegistrationRepository registrationRepository,
           IEventRepository eventRepository,
           IValidator<CreateRegistrationCommand> validator,
           ILogger<CreateRegistrationCommandHandle> logger)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateRegistrationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Регистрация пользователя {UserId} на мероприятие {EventId}",
                request.UserId, request.EventId);

            // Валидация
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Ошибка валидации регистрации: {Errors}", errors);
                return Result.Failure(errors, 400);
            }

            // Проверка что мероприятие существует
            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (@event is null)
            {
                _logger.LogWarning("Мероприятие {EventId} не найдено", request.EventId);
                return Result.Failure($"Мероприятие с Id {request.EventId} не найдено", 404);
            }

            // Проверка что мероприятие опубликовано
            if (!@event.IsPublished)
            {
                _logger.LogWarning("Мероприятие {EventId} не опубликовано", request.EventId);
                return Result.Failure("Нельзя зарегистрироваться на неопубликованное мероприятие", 422);
            }

            // Проверка что мероприятие ещё не началось
            if (@event.Start <= DateTime.UtcNow)
            {
                _logger.LogWarning("Мероприятие {EventId} уже началось", request.EventId);
                return Result.Failure("Нельзя зарегистрироваться на начавшееся мероприятие", 422);
            }

            // Проверка что пользователь ещё не зарегистрирован
            var existing = await _registrationRepository
                .GetRegistrationAsync(request.UserId, request.EventId);

            if (existing is not null)
            {
                // Если регистрация отменена — можно попробовать восстановить
                if (existing.Status == RegistrationStatus.Cancelled)
                {
                    _logger.LogInformation(
                        "Повторная регистрация пользователя {UserId} на {EventId}",
                        request.UserId, request.EventId);

                    existing.Status = RegistrationStatus.Confirmed;
                    existing.ConfirmationDate = DateTime.UtcNow;

                    _registrationRepository.UpdateAsync(existing, cancellationToken);
                    await _registrationRepository.SaveChangesAsync(cancellationToken);

                    return Result.Success();
                }

                _logger.LogWarning(
                    "Пользователь {UserId} уже зарегистрирован на {EventId} (статус {Status})",
                    request.UserId, request.EventId, existing.Status);
                return Result.Failure("Вы уже зарегистрированы на это мероприятие", 409);
            }

            // Проверка лимита участников
            if (@event.MaxParticipants.HasValue)
            {
                var currentCount = await _registrationRepository
                    .CountConfirmedAsync(request.EventId, cancellationToken);

                if (currentCount >= @event.MaxParticipants.Value)
                {
                    _logger.LogWarning(
                        "Мероприятие {EventId} заполнено: {Current}/{Max}",
                        request.EventId, currentCount, @event.MaxParticipants.Value);
                    return Result.Failure("Достигнут лимит участников", 422);
                }
            }

            // регистрируем
            var registration = new Registration
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                UserId = request.UserId,
                Status = RegistrationStatus.Confirmed,
                RegistrationDate = DateTime.UtcNow,
                ConfirmationDate = DateTime.UtcNow
            };

            try
            {
                await _registrationRepository.AddAsync(registration, cancellationToken);
                await _registrationRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Пользователь {UserId} успешно зарегистрирован на {EventId}",
                    request.UserId, request.EventId);

                return Result.Success();
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "Ошибка сохранения регистрации: {Inner}", inner);
                return Result.Failure("Не удалось создать регистрацию. Возможно, вы уже записаны.", 409);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка при регистрации пользователя {UserId} на {EventId}: {Error}",
                    request.UserId, request.EventId, ex.Message);
                return Result.Failure($"Ошибка регистрации: {ex.Message}", 500);
            }
        }
    }
}