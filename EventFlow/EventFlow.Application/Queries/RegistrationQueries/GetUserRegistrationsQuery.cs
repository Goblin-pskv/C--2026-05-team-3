using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.RegistrationQueries
{
    public record GetUserRegistrationsQuery(
        Guid UserId
        ) : IRequest<Result<List<RegistrationDto>>>;
}