using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;

namespace EventFlow.Application.Queries.GetProfileQuery
{
    public record GetProfileQuery(Guid UserId) : IRequest<Result<UserDto>>;
}