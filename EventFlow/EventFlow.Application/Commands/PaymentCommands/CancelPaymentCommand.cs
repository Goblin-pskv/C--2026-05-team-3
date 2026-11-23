using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;

namespace EventFlow.Application.Commands.PaymentCommands
{
    public record CancelPaymentCommand(Guid Id, Guid UserId) : IRequest<Result<PaymentDto>>;
}
