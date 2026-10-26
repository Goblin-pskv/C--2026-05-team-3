using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;

namespace EventFlow.Application.Commands.PaymentCommands
{
    public record InitiatePaymentCommand(Guid TicketId, Guid UserId) : IRequest<Result<PaymentDto>>;
}
