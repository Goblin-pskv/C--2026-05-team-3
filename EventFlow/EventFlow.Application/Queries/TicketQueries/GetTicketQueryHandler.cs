using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.TicketQueries
{
    public class GetTicketQueryHandler : IRequestHandler<GetTicketQuery, Result<TicketDto>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<GetTicketQueryHandler> _logger;

        public GetTicketQueryHandler(
            ITicketRepository ticketRepository,
            ILogger<GetTicketQueryHandler> logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<Result<TicketDto>> Handle(GetTicketQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение билета {Id}", request.Id);

            var ticket = await _ticketRepository.GetByIdWithIncludesAsync(request.Id, ct);
            if (ticket == null)
            {
                _logger.LogWarning("Билет {Id} не найден", request.Id);
                return Result<TicketDto>.Failure("Билет не найден", 404);
            }

            return Result<TicketDto>.Success(new TicketDto
            {
                Id = ticket.Id,
                EventId = ticket.EventId,
                UserId = ticket.UserId,
                TicketNumber = ticket.TicketNumber,
                Status = ticket.status,
                TicketCreationDate = ticket.TicketCreationDate,
                LastModificationDate = ticket.LastModificationDate,
                Price = ticket.Price,
                SeatNumber = ticket.SeatNumber,
                RowNumber = ticket.RowNumber,
                Sector = ticket.Sector
            });
        }
    }
}
