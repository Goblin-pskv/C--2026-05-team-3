using EventFlow.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.AuthCommands
{
    public record ConfirmEmailCommand
        (
        Guid userId,
        string token
        ) : IRequest<Result>;
}