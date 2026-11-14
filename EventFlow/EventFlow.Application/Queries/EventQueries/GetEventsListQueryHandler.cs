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
    public class GetEventsListQueryHandler : IRequestHandler<GetEventsListQuery, Result<List<EventDto>>>
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

        public async Task<Result<List<EventDto>>> Handle(GetEventsListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение списка мероприятий со страницей {Page} размером {PageSize}", 
                request.Page, request.PageSize);

            try
            {
                var events = await _eventRepository.GetEventsWithDetailsAsync(
                    request.Page, 
                    request.PageSize, 
                    request.DateStart ?? DateTime.MinValue, 
                    request.DateEnd ?? DateTime.MaxValue, 
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

                _logger.LogInformation("Успешно получено {Count} мероприятий", dtos.Count);
                return Result<List<EventDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка мероприятий");
                return Result<List<EventDto>>.Failure("Произошла ошибка при получении списка мероприятий", 500);
            }
        }
    }
}