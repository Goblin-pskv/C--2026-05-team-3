using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.RegistrationQueries
{
    public class GetUserRegistrationsQueryHandleMock : IRequestHandler<GetUserRegistrationsQueryMock, Result>
    {
        public Task<Result> Handle(GetUserRegistrationsQueryMock request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}