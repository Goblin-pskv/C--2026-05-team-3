using EventFlow.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.EventCommands
{
    public record CreateEventCommandMock() : IRequest<Result>;
}