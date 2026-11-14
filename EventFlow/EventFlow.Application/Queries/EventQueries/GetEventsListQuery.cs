using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.EventQueries
{
    public record GetEventsListQuery(
        int Page = 0,
        int PageSize = 10,
        DateTime? DateStart = null,
        DateTime? DateEnd = null
        ) : IRequest<Result<List<EventDto>>>;
}