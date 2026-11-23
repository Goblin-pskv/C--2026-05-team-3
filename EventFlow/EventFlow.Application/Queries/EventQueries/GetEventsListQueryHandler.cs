using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.EventQueries
{
    public class GetEventsListQueryHandler : IRequestHandler<GetEventsListQuery, Result<PageResponse<EventDto>>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<GetEventsListQueryHandler> _logger;

        public GetEventsListQueryHandler(
            IEventRepository eventRepository,
            ITicketRepository ticketRepository,
            ILogger<GetEventsListQueryHandler> logger)
        {
            _eventRepository = eventRepository;
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<Result<PageResponse<EventDto>>> Handle(GetEventsListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение списка опубликованных мероприятий со страницей {Page} размером {PageSize}",
                request.Page, request.PageSize);

            try
            {
                var (events, totalCount) = await _eventRepository.GetPublishedEventsAsync(
                    request.Page,
                    request.PageSize,
                    cancellationToken);

                // Если пользователь авторизован — загружаем его активные билеты одним запросом
                HashSet<Guid>? registeredEventIds = null;
                if (request.UserId.HasValue)
                {
                    var tickets = await _ticketRepository.GetByUserAsync(request.UserId.Value, cancellationToken);
                    registeredEventIds = new HashSet<Guid>(tickets
                        .Where(t => t.status == TicketStatus.Confirmed)
                        .Select(t => t.EventId));
                }

                // Загружаем активные билеты для всех событий одной пачкой запросов
                var activeTicketCounts = await _ticketRepository.GetActiveTicketCountByEventIdsAsync(
                    events.Select(e => e.Id).ToList(), cancellationToken);

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
                    ImageUrl = e.ImageUrl,
                    CreatedAt = e.CreatedAt,
                    CurrentParticipants = activeTicketCounts.TryGetValue(e.Id, out var count) ? count : 0,
                    AvailableSpots = e.MaxParticipants.HasValue
                        ? e.MaxParticipants.Value - (activeTicketCounts.TryGetValue(e.Id, out count) ? count : 0)
                        : 0,
                    IsUserRegistered = registeredEventIds != null && registeredEventIds.Contains(e.Id)
                }).ToList();

                var enumTypes = EnumHelper.GetEnumDictionary<EventType>();

                var enumStatuses = EnumHelper.GetEnumDictionary<EventStatus>();

                _logger.LogInformation("Успешно получено {Count} мероприятий из {TotalCount}", dtos.Count, totalCount);
                return Result<PageResponse<EventDto>>.Success(new PageResponse<EventDto>
                {
                    Data = new PaginatedResult<EventDto>
                    {
                        Items = dtos,
                        TotalCount = totalCount
                    },
                    Dictionaries = new List<Dictionary<string, string>> { enumTypes, enumStatuses }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка мероприятий");
                return Result<PageResponse<EventDto>>.Failure("Произошла ошибка при получении списка мероприятий", 500);
            }
        }
    }
}