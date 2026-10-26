using EventFlow.Application.Common;
using MediatR;

namespace EventFlow.Application.Commands.PaymentCommands
{
    public record ConfirmPaymentCommand(Guid Id, Guid UserId) : IRequest<Result>;
}
