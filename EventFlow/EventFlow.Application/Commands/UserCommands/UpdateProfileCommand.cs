using System.Text.Json.Serialization;
using EventFlow.Application.Common;
using MediatR;

namespace EventFlow.Application.Commands.UpdateProfileCommand
{
    public record UpdateProfileCommand
    (
        [property: JsonIgnore] Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        string? PhoneNumber
    ) : IRequest<Result>;
}