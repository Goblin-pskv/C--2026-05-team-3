using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;

namespace EventFlow.Application.Queries.PaymentQueries
{
    public record PaymentStatusQuery(Guid Id, Guid UserId) : IRequest<Result<PaymentDto>>;
}
