using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.PaymentCommands
{
    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, Result<PaymentDto>>
    {
        private readonly ILogger<InitiatePaymentCommandHandler> _logger;

        public InitiatePaymentCommandHandler(ILogger<InitiatePaymentCommandHandler> logger)
        {
            _logger = logger;
        }

        public Task<Result<PaymentDto>> Handle(InitiatePaymentCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Начало оплаты билета {TicketId} пользователем {UserId}", request.TicketId, request.UserId);

            var paymentDto = new PaymentDto()
            {
                Id = Guid.NewGuid(),
                PaymentId = Guid.NewGuid(),
                TicketId = request.TicketId,
                UserId = request.UserId,
                Status = PaymentStatus.Pending,
                PaymentCreationDate = DateTime.UtcNow
            };

            return Task.FromResult(Result<PaymentDto>.Success(paymentDto));
        }
    }
}
