using EventFlow.Application.Common;
using EventFlow.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.EventCommands
{
    public record CreateEventCommandMock(
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
        bool IsPublished
        ) : IRequest<Result>;
}