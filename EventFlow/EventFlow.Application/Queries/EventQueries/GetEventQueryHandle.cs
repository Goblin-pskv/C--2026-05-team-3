using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.EventQueries
{
    public class GetEventQueryHandle : IRequestHandler<GetEventQuery, Result<EventDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GetEventQueryHandle> _logger;

        public GetEventQueryHandle(
            IEventRepository eventRepository,
            ILogger<GetEventQueryHandle> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<Result<EventDto>> Handle(GetEventQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение мероприятия по Id: {EventId}", request.EventId);

            if (request.EventId == Guid.Empty)
            {
                _logger.LogWarning("Передан пустой EventId");
                return Result<EventDto>.Failure("EventId обязателен", 400);
            }

            var eventEntity = await _eventRepository
                .GetByIdWithIncludesAsync(request.EventId, cancellationToken);

            if (eventEntity is null)
            {
                _logger.LogWarning("Мероприятие {EventId} не найдено", request.EventId);
                return Result<EventDto>.Failure($"Мероприятие с Id {request.EventId} не найдено", 404);
            }



            var dto = new EventDto
            {
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                Type = eventEntity.Type.ToString(),
                Start = eventEntity.Start,
                End = eventEntity.End,
                City = eventEntity.City,
                Address = eventEntity.Address,
                Price = eventEntity.Price,
                MaxParticipants = eventEntity.MaxParticipants,
                //OrganizerId = eventEntity.OrganizerId,
                IsPublished = eventEntity.IsPublished,
                CreatedAt = eventEntity.CreatedAt
            };

            _logger.LogInformation("Мероприятие {EventId} успешно получено", request.EventId);
            return Result<EventDto>.Success(dto);
        }
    }
}