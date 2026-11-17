using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.GetProfileQuery
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<UserDto>>
    {
        private readonly IUserRepository _repository;
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IOrganizerRequestRepository _organizerRequestRepository;
        private readonly ILogger<GetProfileQueryHandler> _logger;

        public GetProfileQueryHandler(
            IUserRepository repository,
            IOrganizerRepository organizerRepository,
            IOrganizerRequestRepository organizerRequestRepository,
            ILogger<GetProfileQueryHandler> logger)
        {
            _repository = repository;
            _organizerRepository = organizerRepository;
            _organizerRequestRepository = organizerRequestRepository;
            _logger = logger;
        }

        public async Task<Result<UserDto>> Handle(GetProfileQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение профиля пользователя с ID {UserId}", request.UserId);

            try
            {
                var user = await _repository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Пользователь с ID {UserId} не найден", request.UserId);
                    return Result<UserDto>.Failure("Пользователь не найден", 404);
                }

                var userRoles = await _repository.GetUserRoles(request.UserId);
                var role = userRoles.FirstOrDefault() ?? string.Empty;

                var dto = new UserDto
                {
                    FullName = user.FullName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = role
                };

                // Проверяем, есть ли у пользователя подтверждённый профиль организатора
                var organizer = await _organizerRepository.GetByUserIdAsync(request.UserId, ct);
                if (organizer != null)
                {
                    dto.OrganizerProfile = new OrganizerDto
                    {
                        CompanyName = organizer.CompanyName,
                        Description = organizer.Description,
                        WebSite = organizer.WebSite,
                        IsVerified = true,
                        EventsCount = organizer.EventsCount
                    };
                }
                else
                {
                    // Проверяем, есть ли активная заявка на организатора
                    var requestEntity = await _organizerRequestRepository.GetActiveByUserIdAsync(request.UserId, ct);
                    if (requestEntity != null)
                    {
                        dto.OrganizerProfile = new OrganizerDto
                        {
                            IsVerified = false,
                            EventsCount = 0
                        };
                    }
                }

                _logger.LogInformation("Профиль пользователя с ID {UserId} успешно получен", request.UserId);
                return Result<UserDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении профиля пользователя с ID {UserId}", request.UserId);
                return Result<UserDto>.Failure("Произошла ошибка при получении профиля пользователя", 500);
            }
        }
    }
}