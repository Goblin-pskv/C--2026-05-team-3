using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;

namespace EventFlow.Application.Commands.RegisterCommand
{
    public record RegisterUserCommand
    (
        string UserName,
        string FirstName,
        string LastName,
        string Email,
        string PasswordHash,
        string PhoneNumber
    ) : IRequest<Result>;
}