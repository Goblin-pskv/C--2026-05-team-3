using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.PaymentQueries
{
    public class PaymentStatusQueryHandler : IRequestHandler<PaymentStatusQuery, Result<PaymentDto>>
    {
        private readonly ILogger<PaymentStatusQueryHandler> _logger;

        public PaymentStatusQueryHandler(ILogger<PaymentStatusQueryHandler> logger)
        {
            _logger = logger;
        }

        public Task<Result<PaymentDto>> Handle(PaymentStatusQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение статуса платежа {Id} пользователя {UserId}", request.Id, request.UserId);

            var paymentDto = new PaymentDto()
            {
                Id = request.Id,
                PaymentId = request.Id,
                UserId = request.UserId,
                Status = PaymentStatus.Pending,
                PaymentCreationDate = DateTime.UtcNow
            };

            return Task.FromResult(Result<PaymentDto>.Success(paymentDto));
        }
    }
}
