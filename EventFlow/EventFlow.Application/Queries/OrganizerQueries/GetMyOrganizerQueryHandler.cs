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
        private readonly IOrganizerRequestRepository _organizerRequestRepository;
        private readonly ILogger<GetMyOrganizerQueryHandler> _logger;

        public GetMyOrganizerQueryHandler(IOrganizerRepository organizerRepository, IOrganizerRequestRepository organizerRequestRepository, ILogger<GetMyOrganizerQueryHandler> logger)
        {
            _organizerRepository = organizerRepository;
            _organizerRequestRepository = organizerRequestRepository;
            _logger = logger;
        }

        public async Task<Result<OrganizerDto>> Handle(GetMyOrganizerQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение профиля организатора для текущего пользователя");

            var userId = request.userId;

            try
            {
                // Сначала проверяем, есть ли подтверждённый профиль
                var organizer = await _organizerRepository.GetByUserIdAsync((Guid)userId, ct);
                if (organizer != null)
                {
                    var dto = new OrganizerDto()
                    {
                        CompanyName = organizer.CompanyName,
                        Description = organizer.Description,
                        WebSite = organizer.WebSite,
                        IsVerified = true,
                        EventsCount = organizer.EventsCount
                    };
                    _logger.LogInformation("Профиль организатора для пользователя {UserId} успешно получен", userId);
                    return Result<OrganizerDto>.Success(dto);
                }

                // Если профиля нет, проверяем активную заявку
                var requestEntity = await _organizerRequestRepository.GetActiveByUserIdAsync((Guid)userId, ct);
                if (requestEntity != null)
                {
                    var dto = new OrganizerDto()
                    {
                        IsVerified = false,
                        EventsCount = 0
                    };
                    _logger.LogInformation("Для пользователя {UserId} найдена активная заявка на организатора", userId);
                    return Result<OrganizerDto>.Success(dto);
                }

                _logger.LogWarning("Профиль организатора для пользователя {UserId} не найден", userId);
                return Result<OrganizerDto>.Failure("Профиль организатора не найден", 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении профиля организатора для пользователя {UserId}", userId);
                return Result<OrganizerDto>.Failure("Ошибка при получении профиля организатора", 500);
            }
        }
    }
}