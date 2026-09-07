
using EventFlow.API.Controllers;
using EventFlow.Application.Common;
using MediatR;

namespace EventFlow.Application.Commands.EventCommands
{
    public class UpdateEventCommandHandleMock : IRequestHandler<UpdateEventCommandMock, Result>
    {
        public Task<Result> Handle(UpdateEventCommandMock request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}