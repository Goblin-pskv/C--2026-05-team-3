using EventFlow.Application.Common;
using EventFlow.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace EventFlow.Application.Queries.OrganizerQueries
{
    public record GetAllOrganizerQuery(int offset, int count) : IRequest<Result<IEnumerable<Organizer>>>;
}