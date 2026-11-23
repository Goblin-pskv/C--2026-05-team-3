using EventFlow.Application.Common;
using MediatR;
using System;

namespace EventFlow.Application.Commands.TicketCommands
{
    public record CreateTicketCommand(Guid EventId, Guid UserId) : IRequest<Result<Guid>>;
}
