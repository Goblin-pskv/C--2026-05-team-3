using EventFlow.Application.Common;
using MediatR;

namespace EventFlow.Application.Commands.OrganizerCommands
{
    public record ApproveOrganizerCommand(Guid Id, Guid? AdminId) : IRequest<Result>;
}