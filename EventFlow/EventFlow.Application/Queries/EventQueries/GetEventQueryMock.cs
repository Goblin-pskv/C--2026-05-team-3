using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.EventQueries
{
    public record GetEventQueryMock(Guid EventId) : IRequest<Result<EventDto>>;
}