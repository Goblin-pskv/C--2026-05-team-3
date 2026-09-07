using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.EventCommands
{
    public class CreateEventCommandHandleMock : IRequestHandler<RegisterUserCommand, Result>
    {
        public Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}