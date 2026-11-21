using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFlow.Application.Queries.EventQueries
{
    public class GetEventsListQueryHandler : IRequestHandler<GetEventsListQuery, Result<PaginatedResult<EventDto>>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GetEventsListQueryHandler> _logger;

        public GetEventsListQueryHandler(
            IEventRepository eventRepository,
            ILogger<GetEventsListQueryHandler> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<EventDto>>> Handle(GetEventsListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение списка опубликованных мероприятий со страницей {Page} размером {PageSize}",
                request.Page, request.PageSize);

            try
            {
                var (events, totalCount) = await _eventRepository.GetPublishedEventsAsync(
                    request.Page,
                    request.PageSize,
                    cancellationToken);

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
                    CreatedAt = e.CreatedAt
                }).ToList();

                _logger.LogInformation("Успешно получено {Count} мероприятий из {TotalCount}", dtos.Count, totalCount);
                return Result<PaginatedResult<EventDto>>.Success(new PaginatedResult<EventDto>
                {
                    Items = dtos,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка мероприятий");
                return Result<PaginatedResult<EventDto>>.Failure("Произошла ошибка при получении списка мероприятий", 500);
            }
        }
    }
}