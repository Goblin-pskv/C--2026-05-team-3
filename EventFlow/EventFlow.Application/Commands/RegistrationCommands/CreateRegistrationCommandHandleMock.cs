using EventFlow.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.RegistrationCommands
{
    public class CreateRegistrationCommandHandleMock : IRequestHandler<CreateRegistrationCommandMock, Result>
    {
        public Task<Result> Handle(CreateRegistrationCommandMock request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}