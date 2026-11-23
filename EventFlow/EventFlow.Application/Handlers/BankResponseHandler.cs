using EventFlow.Application.Interfaces;
using EventFlow.Application.Messages;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Handlers
{
    public class BankResponseHandler
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<BankResponseHandler> _logger;

        public BankResponseHandler(
            IPaymentRepository paymentRepository,
            ITicketRepository ticketRepository,
            ILogger<BankResponseHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task Handle(BankResponseSimulated msg, CancellationToken ct)
        {
            _logger.LogInformation("Обработка BankResponseSimulated для платежа {PaymentId}", msg.PaymentId);

            var payment = await _paymentRepository.GetByIdWithIncludesAsync(msg.PaymentId, ct);
            if (payment == null)
            {
                _logger.LogWarning("Платёж {PaymentId} не найден при обработке BankResponseSimulated", msg.PaymentId);
                return;
            }

            if (payment.Status == PaymentStatus.Confirmed)
            {
                _logger.LogInformation("Платёж {PaymentId} уже Confirmed — идемпотентность, выходим", msg.PaymentId);
                return;
            }

            payment.Status = PaymentStatus.Confirmed;
            payment.PaidAt = DateTime.UtcNow;
            payment.LastModificationDate = DateTime.UtcNow;

            var ticket = payment.Ticket;
            if (ticket != null && ticket.status != TicketStatus.Confirmed)
            {
                ticket.status = TicketStatus.Confirmed;
                ticket.LastModificationDate = DateTime.UtcNow;
            }

            await SaveChangesAsync(payment, ticket, ct);

            _logger.LogInformation("Платёж {PaymentId} переведён в Confirmed, билет {TicketId} также подтверждён", msg.PaymentId, payment.TicketId);
        }

        private async Task SaveChangesAsync(Payment payment, Tickets? ticket, CancellationToken ct)
        {
            await _paymentRepository.UpdateAsync(payment, ct);
            if (ticket != null)
                await _ticketRepository.UpdateAsync(ticket, ct);
            await _paymentRepository.SaveChangesAsync(ct);
        }
    }
}
