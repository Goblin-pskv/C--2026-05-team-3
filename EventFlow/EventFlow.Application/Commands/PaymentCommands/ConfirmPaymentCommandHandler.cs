using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Application.Messages;
using EventFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace EventFlow.Application.Commands.PaymentCommands
{
    public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, Result<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMessageBus _messageBus;
        private readonly ILogger<ConfirmPaymentCommandHandler> _logger;

        public ConfirmPaymentCommandHandler(
            IPaymentRepository paymentRepository,
            IMessageBus messageBus,
            ILogger<ConfirmPaymentCommandHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _messageBus = messageBus;
            _logger = logger;
        }

        public async Task<Result<PaymentDto>> Handle(ConfirmPaymentCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Подтверждение платежа {Id} пользователем {UserId}", request.Id, request.UserId);

            var payment = await _paymentRepository.GetByIdWithIncludesAsync(request.Id, ct);
            if (payment == null)
            {
                _logger.LogWarning("Платёж {Id} не найден", request.Id);
                return Result<PaymentDto>.Failure("Платёж не найден", 404);
            }

            if (payment.UserId != request.UserId)
            {
                _logger.LogWarning("Попытка подтвердить чужой платёж: платёж {Id}, пользователь {UserId}", request.Id, request.UserId);
                return Result<PaymentDto>.Failure("Платёж не принадлежит пользователю", 403);
            }

            if (payment.Status != PaymentStatus.Pending)
            {
                _logger.LogWarning("Платёж {Id} уже в статусе {Status}", request.Id, payment.Status);
                return Result<PaymentDto>.Failure($"Платёж уже в статусе {payment.Status}", 409);
            }

            payment.Status = PaymentStatus.Processing;
            payment.LastModificationDate = DateTime.UtcNow;
            await _paymentRepository.UpdateAsync(payment, ct);
            await _paymentRepository.SaveChangesAsync(ct);

            await _messageBus.ScheduleAsync(
                new BankResponseSimulated(payment.Id),
                TimeSpan.FromSeconds(15));

            _logger.LogInformation("Платёж {Id} переведён в статус Processing, отложено сообщение BankResponseSimulated", payment.Id);

            return Result<PaymentDto>.Success(new PaymentDto
            {
                Id = payment.Id,
                TicketId = payment.TicketId,
                UserId = payment.UserId,
                Price = payment.Price,
                Status = EnumHelper.GetDescription(PaymentStatus.Processing),
                PaymentCreationDate = payment.PaymentCreationDate,
                LastModificationDate = payment.LastModificationDate
            });
        }
    }
}
