using EventFlow.Application.Common;
using MediatR;

namespace EventFlow.Application.Queries.GetProfileQuery
{
    public record GetProfileQuery(Guid UserId) : IRequest<Result>;
}