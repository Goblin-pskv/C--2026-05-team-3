using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.EventQueries
{
    public class GetMyEventsQueryHandler : IRequestHandler<GetMyEventsQuery, Result<PaginatedResult<EventDto>>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GetMyEventsQueryHandler> _logger;

        public GetMyEventsQueryHandler(IEventRepository eventRepository, ILogger<GetMyEventsQueryHandler> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<EventDto>>> Handle(GetMyEventsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение списка мероприятий организатора {OrganizerId} со страницей {Page} размером {PageSize}",
                request.OrganizerId, request.Page, request.PageSize);

            try
            {
                var (events, totalCount) = await _eventRepository.GetEventsByOrganizerWithPaginationAsync(
                    request.OrganizerId, request.Page, request.PageSize, cancellationToken);

                var dtos = events.Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Type = e.Type.ToString(),
                    Start = e.Start,
                    End = e.End,
                    City = e.City,
                    Address = e.Address,
                    Price = e.Price,
                    MaxParticipants = e.MaxParticipants,
                    IsPublished = e.IsPublished,
                    Status = e.Status.ToString(),
                    CreatedAt = e.CreatedAt
                }).ToList();

                var result = new PaginatedResult<EventDto>
                {
                    Items = dtos,
                    TotalCount = totalCount
                };

                _logger.LogInformation("Успешно получено {Count} мероприятий из {TotalCount}", dtos.Count, totalCount);
                return Result<PaginatedResult<EventDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка мероприятий организатора {OrganizerId}", request.OrganizerId);
                return Result<PaginatedResult<EventDto>>.Failure("Произошла ошибка при получении списка мероприятий", 500);
            }
        }
    }
}
