using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;

namespace EventFlow.Application.Commands.PaymentCommands
{
    public record ConfirmPaymentCommand(Guid Id, Guid UserId) : IRequest<Result<PaymentDto>>;
}
