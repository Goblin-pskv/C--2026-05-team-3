using EventFlow.Application.Common;
using MediatR;

namespace EventFlow.Application.Commands.OrganizerCommands
{
    public record CreateOrganizerCommand(Guid userId, string? CompanyName, string? Description, string? WebSite) : IRequest<Result>;
}