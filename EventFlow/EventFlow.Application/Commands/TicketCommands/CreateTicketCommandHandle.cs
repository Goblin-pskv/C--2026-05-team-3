using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.TicketCommands
{
    public class CreateTicketCommandHandle : IRequestHandler<CreateTicketCommand, Result<Guid>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<CreateTicketCommandHandle> _logger;

        public CreateTicketCommandHandle(
            ITicketRepository ticketRepository,
            IEventRepository eventRepository,
            ILogger<CreateTicketCommandHandle> logger)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreateTicketCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Создание билета для пользователя {UserId} на мероприятие {EventId}",
                request.UserId, request.EventId);

            var existing = await _ticketRepository.ExistsAsync(request.UserId, request.EventId, ct);
            if (existing)
            {
                _logger.LogWarning("Билет уже существует для пользователя {UserId} на мероприятие {EventId}",
                    request.UserId, request.EventId);
                return Result<Guid>.Failure("Билет уже существует", 409);
            }

            var @event = await _eventRepository.GetByIdAsync(request.EventId, ct);
            if (@event == null)
            {
                _logger.LogWarning("Мероприятие {EventId} не найдено", request.EventId);
                return Result<Guid>.Failure("Мероприятие не найдено", 404);
            }

            var count = await _ticketRepository.GetByEventAsync(request.EventId, ct);
            var sequence = count.Count + 1;
            var ticketNumber = $"EVT-{@event.Start.Year}-{sequence:D6}";

            var ticket = new Tickets
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                UserId = request.UserId,
                TicketNumber = ticketNumber,
                Price = @event.Price,
                status = TicketStatus.Pending,
                TicketCreationDate = DateTime.UtcNow
            };

            await _ticketRepository.AddAsync(ticket, ct);
            await _ticketRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Билет {TicketNumber} ({Id}) создан для пользователя {UserId}",
                ticketNumber, ticket.Id, request.UserId);

            return Result<Guid>.Success(ticket.Id);
        }
    }
}
