using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;

namespace EventFlow.Application.Queries.PaymentQueries
{
    public record PaymentsQuery(Guid UserId) : IRequest<Result<List<PaymentDto>>>;
}
