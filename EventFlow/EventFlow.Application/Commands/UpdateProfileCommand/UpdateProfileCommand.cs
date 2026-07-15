using EventFlow.Application.Common;
using EventFlow.Domain.Enums;
using MediatR;

namespace EventFlow.Application.Commands.UpdateProfileCommand
{
    public record UpdateProfileCommand
    (
        Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        string? PhoneNumber
    ) : IRequest<Result>;
}