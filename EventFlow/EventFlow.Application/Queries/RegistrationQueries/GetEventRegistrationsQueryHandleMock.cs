using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.RegistrationQueries
{
    public class GetEventRegistrationsQueryHandleMock : IRequestHandler<GetEventRegistrationsQueryMock, Result>
    {
        public Task<Result> Handle(GetEventRegistrationsQueryMock request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}