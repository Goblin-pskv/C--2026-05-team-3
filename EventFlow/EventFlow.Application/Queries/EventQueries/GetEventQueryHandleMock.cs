using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;

namespace EventFlow.Application.Queries.EventQueries
{
    public class GetEventQueryHandleMock : IRequestHandler<GetEventQueryMock, Result<EventDto>>
    {
        public Task<Result<EventDto>> Handle(GetEventQueryMock request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}