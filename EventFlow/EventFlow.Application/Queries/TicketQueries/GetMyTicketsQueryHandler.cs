using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.TicketQueries
{
    public class GetMyTicketsQueryHandler : IRequestHandler<GetMyTicketsQuery, Result<List<TicketDto>>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<GetMyTicketsQueryHandler> _logger;

        public GetMyTicketsQueryHandler(
            ITicketRepository ticketRepository,
            ILogger<GetMyTicketsQueryHandler> logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<Result<List<TicketDto>>> Handle(GetMyTicketsQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение списка билетов пользователя {UserId}", request.UserId);

            var tickets = await _ticketRepository.GetByUserAsync(request.UserId, ct);

            var dtos = tickets.Select(t => new TicketDto
            {
                Id = t.Id,
                EventId = t.EventId,
                UserId = t.UserId,
                TicketNumber = t.TicketNumber,
                Status = t.status,
                TicketCreationDate = t.TicketCreationDate,
                LastModificationDate = t.LastModificationDate,
                Price = t.Price,
                SeatNumber = t.SeatNumber,
                RowNumber = t.RowNumber,
                Sector = t.Sector
            }).ToList();

            return Result<List<TicketDto>>.Success(dtos);
        }
    }
}
