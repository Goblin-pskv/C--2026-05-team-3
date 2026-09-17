using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.RegistrationQueries
{
    public record GetEventRegistrationsQuery(
        Guid EventId
        ) : IRequest<Result<List<RegistrationDto>>>;
   
}