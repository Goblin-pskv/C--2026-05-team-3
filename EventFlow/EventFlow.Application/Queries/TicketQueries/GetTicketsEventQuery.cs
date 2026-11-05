using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.TicketQueries
{
    public record GetTicketsEventQuery(Guid EventId) : IRequest<Result<List<TicketDto>>>;    
}
