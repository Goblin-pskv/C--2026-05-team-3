using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.RegistrationCommands
{
    public class CancelRegistrationCommandHandleMock : IRequestHandler<CancelRegistrationCommandMock, Result>
    {
        public Task<Result> Handle(CancelRegistrationCommandMock request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}