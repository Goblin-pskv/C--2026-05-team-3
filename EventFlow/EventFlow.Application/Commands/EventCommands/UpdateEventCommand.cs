using EventFlow.Application.Common;
using EventFlow.Domain.Enums;
using MediatR;

namespace EventFlow.API.Controllers
{
    public record UpdateEventCommand(
        Guid EventId,
        string Title,
        string Description,
        EventType Type,
        DateTime Start,
        DateTime End,
        string City,
        string Address,
        decimal Price,
        int MaxParticipants,
        Guid OrganizerId,
        bool IsPublished) : IRequest<Result>;
      
}