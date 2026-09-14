using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.EventQueries
{
    public record GetEventQueryMock() : IRequest<Result<EventDto>>
    {
    public Guid EventId { get; init; }
    }
}