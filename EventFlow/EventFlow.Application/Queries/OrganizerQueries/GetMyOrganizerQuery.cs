using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Domain.Entities;
using MediatR;

namespace EventFlow.Application.Queries.OrganizerQueries
{
    public record GetMyOrganizerQuery(Guid userId) : IRequest<Result<OrganizerDto>>;
}