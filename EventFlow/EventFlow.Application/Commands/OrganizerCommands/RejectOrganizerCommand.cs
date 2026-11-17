using EventFlow.Application.Common;
using MediatR;

namespace EventFlow.Application.Commands.OrganizerCommands
{
    public record RejectOrganizerCommand(Guid Id, string? Reason, Guid? AdminId) : IRequest<Result>;
}