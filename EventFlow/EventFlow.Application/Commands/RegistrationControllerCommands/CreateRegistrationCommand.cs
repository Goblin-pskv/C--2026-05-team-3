using EventFlow.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.RegistrationCommands
{
    public record CreateRegistrationCommand(
        Guid EventId,
        Guid UserId
        ) : IRequest<Result>;
}