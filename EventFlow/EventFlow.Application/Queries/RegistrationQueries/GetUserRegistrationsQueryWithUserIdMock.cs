using EventFlow.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.RegistrationQueries
{
    public record GetUserRegistrationsQueryWithUserIdMock(Guid UserId) : IRequest<Result>
    {
    }
}