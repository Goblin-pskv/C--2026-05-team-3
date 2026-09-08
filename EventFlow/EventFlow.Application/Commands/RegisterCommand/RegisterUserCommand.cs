using EventFlow.Application.Common;
using MediatR;

namespace EventFlow.Application.Commands.RegisterCommand
{
    public record RegisterUserCommand
    (
        string UserName,
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string PhoneNumber
    ) : IRequest<Result>;
}