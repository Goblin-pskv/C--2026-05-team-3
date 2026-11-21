using EventFlow.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EventFlow.Application.Commands.EventCommands
{
    public record UploadEventImageCommand(
        Guid EventId,
        IFormFile Image,
        Guid UserId
    ) : IRequest<Result>;
}
