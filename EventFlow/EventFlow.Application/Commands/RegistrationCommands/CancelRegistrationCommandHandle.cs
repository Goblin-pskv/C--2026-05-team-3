using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Enums;
using EventFlow.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.RegistrationCommands
{
    public class CancelRegistrationCommandHandleMock
        : IRequestHandler<CancelRegistrationCommand, Result>
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IValidator<CancelRegistrationCommand> _validator;
        private readonly ILogger<CancelRegistrationCommandHandleMock> _logger;

        public CancelRegistrationCommandHandleMock(
            IRegistrationRepository registrationRepository,
            IValidator<CancelRegistrationCommand> validator,
            ILogger<CancelRegistrationCommandHandleMock> logger)
        {
            _registrationRepository = registrationRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result> Handle(
            CancelRegistrationCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Отмена регистрации пользователя {UserId} на мероприятие {EventId}",
                request.UserId, request.EventId);

            // Валидация
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Ошибка валидации CancelRegistration: {Errors}", errors);
                return Result.Failure(errors, 400);
            }

            // Ищем регистрацию
            var registration = await _registrationRepository
                .GetRegistrationAsync(request.UserId, request.EventId, cancellationToken);

            if (registration is null)
            {
                _logger.LogWarning(
                    "Регистрация пользователя {UserId} на мероприятие {EventId} не найдена",
                    request.UserId, request.EventId);
                return Result.Failure("Регистрация не найдена", 404);
            }
                     
            if (registration.Status == RegistrationStatus.Cancelled)
            {
                _logger.LogWarning(
                    "Регистрация {RegistrationId} уже отменена", registration.Id);
                return Result.Failure("Регистрация уже отменена", 409);
            }

            try
            {
                registration.Cancel();
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(
                    "Отмена регистрации {RegistrationId} запрещена: {Reason}",
                    registration.Id, ex.Message);
                return Result.Failure(ex.Message, 422);
            }

            try
            {
                _registrationRepository.UpdateAsync(registration, cancellationToken);
                await _registrationRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Регистрация {RegistrationId} успешно отменена", registration.Id);

                return Result.Success();
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "Ошибка сохранения при отмене регистрации: {Inner}", inner);
                return Result.Failure($"Ошибка сохранения: {inner}", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка при отмене регистрации {RegistrationId}: {Error}",
                    registration.Id, ex.Message);
                return Result.Failure($"Ошибка отмены регистрации: {ex.Message}", 500);
            }
        }
    }
}