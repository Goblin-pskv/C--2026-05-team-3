using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.OrganizerQueries
{
    public class GetMyOrganizerQueryHandler : IRequestHandler<GetMyOrganizerQuery, Result<OrganizerDto>>
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetMyOrganizerQueryHandler> _logger;

        public GetMyOrganizerQueryHandler(IOrganizerRepository organizerRepository, IUserRepository userRepository, ILogger<GetMyOrganizerQueryHandler> logger)
        {
            _organizerRepository = organizerRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Result<OrganizerDto>> Handle(GetMyOrganizerQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение профиля организатора для текущего пользователя");
            
            var userId = request.userId;

            try
            {
                var organizer = await _organizerRepository.GetByUserIdAsync((Guid)userId, ct);
                if (organizer == null)
                {
                    _logger.LogWarning("Профиль организатора для пользователя {UserId} не найден", userId);
                    return Result<OrganizerDto>.Failure("Профиль организатора не найден", 404);
                }
                var organizerDto = new OrganizerDto()
                {
                    CompanyName = organizer.CompanyName,
                    Description = organizer.Description,
                    WebSite = organizer.WebSite,
                    IsVerified = organizer.IsVerified,
                    EventsCount = organizer.EventsCount
                };

                _logger.LogInformation("Профиль организатора для пользователя {UserId} успешно получен", userId);
                return Result<OrganizerDto>.Success(organizerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении профиля организатора для пользователя {UserId}", userId);
                return Result<OrganizerDto>.Failure("Ошибка при получении профиля организатора", 500);
            }
        }
    }
}