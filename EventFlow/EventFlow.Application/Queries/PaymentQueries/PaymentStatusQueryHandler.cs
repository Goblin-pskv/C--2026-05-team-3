using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.PaymentQueries
{
    public class PaymentStatusQueryHandler : IRequestHandler<PaymentStatusQuery, Result<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentStatusQueryHandler> _logger;

        public PaymentStatusQueryHandler(
            IPaymentRepository paymentRepository,
            ILogger<PaymentStatusQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<Result<PaymentDto>> Handle(PaymentStatusQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение статуса платежа {Id} пользователя {UserId}", request.Id, request.UserId);

            var payment = await _paymentRepository.GetByIdWithIncludesAsync(request.Id, ct);
            if (payment == null)
            {
                _logger.LogWarning("Платёж {Id} не найден", request.Id);
                return Result<PaymentDto>.Failure("Платёж не найден", 404);
            }

            if (payment.UserId != request.UserId)
            {
                _logger.LogWarning("Попытка запросить чужой платёж: платёж {Id}, пользователь {UserId}", request.Id, request.UserId);
                return Result<PaymentDto>.Failure("Платёж не принадлежит пользователю", 403);
            }

            return Result<PaymentDto>.Success(new PaymentDto
            {
                Id = payment.Id,
                TicketId = payment.TicketId,
                UserId = payment.UserId,
                Price = payment.Price,
                Status = payment.Status,
                PaymentCreationDate = payment.PaymentCreationDate,
                LastModificationDate = payment.LastModificationDate,
                PaidAt = payment.PaidAt
            });
        }
    }
}
