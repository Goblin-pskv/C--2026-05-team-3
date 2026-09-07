using EventFlow.Application.Common;
using EventFlow.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.RegistrationQueries
{
    public record GetUserRegistrationsQueryMock(Guid UserId) : IRequest<Result>;
}