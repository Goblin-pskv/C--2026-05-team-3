using EventFlow.Application.Common;
using MediatR;

namespace EventFlow.API.Controllers
{
    public record UpdateEventCommandMock(Guid EventId) : IRequest<Result>;
}