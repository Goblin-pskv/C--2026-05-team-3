using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;

namespace EventFlow.Application.Queries.EventQueries
{
    public record GetMyEventsQuery(Guid OrganizerId, int Page = 0, int PageSize = 10) : IRequest<Result<PageResponse<EventDto>>>;
}
