using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.PaymentQueries
{
    public class PaymentsQueryHandler : IRequestHandler<PaymentsQuery, Result<List<PaymentDto>>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentsQueryHandler> _logger;

        public PaymentsQueryHandler(
            IPaymentRepository paymentRepository,
            ILogger<PaymentsQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<Result<List<PaymentDto>>> Handle(PaymentsQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение истории платежей пользователя {UserId}", request.UserId);

            var payments = await _paymentRepository.GetByUserAsync(request.UserId, ct);

            var dtos = payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                TicketId = p.TicketId,
                UserId = p.UserId,
                Price = p.Price,
                Status = EnumHelper.GetDescription(p.Status),
                PaymentCreationDate = p.PaymentCreationDate,
                LastModificationDate = p.LastModificationDate,
                PaidAt = p.PaidAt
            }).ToList();

            return Result<List<PaymentDto>>.Success(dtos);
        }
    }
}
