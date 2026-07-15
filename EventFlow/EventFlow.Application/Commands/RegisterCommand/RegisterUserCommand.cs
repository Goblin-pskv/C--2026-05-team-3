using EventFlow.Application.Common;
using MediatR;

namespace EventFlow.Application.Commands.RegisterCommand
{
    public record RegisterUserCommand
    (
        string FirstName,
        string LastName,
        string Email,
        string PasswordHash,
        string PhoneNumber
    ) : IRequest<Result>;
}