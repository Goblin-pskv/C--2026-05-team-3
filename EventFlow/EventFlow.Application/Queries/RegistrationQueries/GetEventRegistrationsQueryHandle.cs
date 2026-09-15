using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.RegistrationQueries
{
    public class GetEventRegistrationsQueryHandle
        : IRequestHandler<GetEventRegistrationsQuery, Result<List<RegistrationDto>>>
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IValidator<GetEventRegistrationsQuery> _validator;
        private readonly ILogger<GetEventRegistrationsQueryHandle> _logger;

        public GetEventRegistrationsQueryHandle(
            IRegistrationRepository registrationRepository,
            IEventRepository eventRepository,
            IValidator<GetEventRegistrationsQuery> validator,
            ILogger<GetEventRegistrationsQueryHandle> logger)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<List<RegistrationDto>>> Handle(
            GetEventRegistrationsQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Получение участников мероприятия {EventId}", request.EventId);

            // Валидация
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Ошибка валидации GetEventRegistrations: {Errors}", errors);
                return Result<List<RegistrationDto>>.Failure(errors, 400);
            }

            // Проверка что мероприятие существует
            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (@event is null)
            {
                _logger.LogWarning("Мероприятие {EventId} не найдено", request.EventId);
                return Result<List<RegistrationDto>>.Failure(
                    $"Мероприятие с Id {request.EventId} не найдено", 404);
            }

            var registrations = await _registrationRepository
                .GetRegistrationsByEventIdAsync(request.EventId, cancellationToken);

            var dtos = registrations
                .Select(r => new RegistrationDto
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    EventTitle = @event.Title,
                    EventStart = @event.Start,

                    UserId = r.UserId,
                    UserName = r.User?.FullName ?? string.Empty,
                    UserEmail = r.User?.Email ?? string.Empty,

                    Status = r.Status.ToString(),
                    RegistrationDate = r.RegistrationDate,
                    ConfirmationDate = r.ConfirmationDate
                })
                .ToList();

            _logger.LogInformation(
                "Мероприятие {EventId}: получено {Count} участников",
                request.EventId, dtos.Count);

            return Result<List<RegistrationDto>>.Success(dtos);
        }
    }
}