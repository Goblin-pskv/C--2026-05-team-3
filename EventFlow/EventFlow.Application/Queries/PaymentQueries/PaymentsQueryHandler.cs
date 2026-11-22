using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.PaymentQueries
{
    public class PaymentsQueryHandler : IRequestHandler<PaymentsQuery, Result<List<PaymentDto>>>
    {
        private readonly ILogger<PaymentsQueryHandler> _logger;

        public PaymentsQueryHandler(ILogger<PaymentsQueryHandler> logger)
        {
            _logger = logger;
        }

        public Task<Result<List<PaymentDto>>> Handle(PaymentsQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение истории платежей пользователя {UserId}", request.UserId);

            return Task.FromResult(Result<List<PaymentDto>>.Success(new List<PaymentDto>()));
        }
    }
}
