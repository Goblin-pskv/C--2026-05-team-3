using EventFlow.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.PaymentCommands
{
    public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, Result>
    {
        private readonly ILogger<ConfirmPaymentCommandHandler> _logger;

        public ConfirmPaymentCommandHandler(ILogger<ConfirmPaymentCommandHandler> logger)
        {
            _logger = logger;
        }

        public Task<Result> Handle(ConfirmPaymentCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Подтверждение платежа {Id} пользователем {UserId}", request.Id, request.UserId);

            return Task.FromResult(Result.Success(200, "Оплата подтверждена"));
        }
    }
}
