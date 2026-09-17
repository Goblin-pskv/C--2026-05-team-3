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
    public class GetUserRegistrationsQueryHandle : IRequestHandler<GetUserRegistrationsQuery, Result<List<RegistrationDto>>>
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IValidator<GetUserRegistrationsQuery> _validator;
        private readonly ILogger<GetUserRegistrationsQueryHandle> _logger;

        public GetUserRegistrationsQueryHandle(
            IRegistrationRepository registrationRepository,
            IValidator<GetUserRegistrationsQuery> validator,
            ILogger<GetUserRegistrationsQueryHandle> logger)
        {
            _registrationRepository = registrationRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<List<RegistrationDto>>> Handle(
            GetUserRegistrationsQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Получение регистраций пользователя {UserId}", request.UserId);

            // Валидация
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Ошибка валидации GetUserRegistrations: {Errors}", errors);
                return Result<List<RegistrationDto>>.Failure(errors, 400);
            }

            // Загружаем регистрации вместе с Event и User
            var registrations = await _registrationRepository
                .GetRegistrationsByUserIdAsync(request.UserId, cancellationToken);

            
            var dtos = registrations
                .Select(r => new RegistrationDto
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    EventTitle = r.Event?.Title ?? string.Empty,
                    EventStart = r.Event?.Start ?? default,

                    UserId = r.UserId,
                    UserName = r.User?.FullName ?? string.Empty,
                    UserEmail = r.User?.Email ?? string.Empty,

                    Status = r.Status.ToString(),
                    RegistrationDate = r.RegistrationDate,
                    ConfirmationDate = r.ConfirmationDate
                })
                .ToList();

            _logger.LogInformation(
                "Пользователь {UserId}: получено {Count} регистраций",
                request.UserId, dtos.Count);

            return Result<List<RegistrationDto>>.Success(dtos);
        }
    }
}