using EventFlow.Application.Common;
using MediatR;
using System;

namespace EventFlow.Application.Commands.RegistrationCommands
{
    public record CreateRegistrationCommand(
        Guid EventId,
        Guid UserId
        ) : IRequest<Result<Guid>>;
}