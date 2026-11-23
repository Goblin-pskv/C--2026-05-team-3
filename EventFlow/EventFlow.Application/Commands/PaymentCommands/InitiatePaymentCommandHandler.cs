using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.PaymentCommands
{
    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, Result<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<InitiatePaymentCommandHandler> _logger;

        public InitiatePaymentCommandHandler(
            IPaymentRepository paymentRepository,
            ITicketRepository ticketRepository,
            ILogger<InitiatePaymentCommandHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<Result<PaymentDto>> Handle(InitiatePaymentCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Начало оплаты билета {TicketId} пользователем {UserId}", request.TicketId, request.UserId);

            var ticket = await _ticketRepository.GetByIdWithIncludesAsync(request.TicketId, ct);
            if (ticket == null)
            {
                _logger.LogWarning("Билет {TicketId} не найден", request.TicketId);
                return Result<PaymentDto>.Failure("Билет не найден", 404);
            }

            if (ticket.UserId != request.UserId)
            {
                _logger.LogWarning("Попытка оплаты чужого билета: билет {TicketId}, пользователь {UserId}", request.TicketId, request.UserId);
                return Result<PaymentDto>.Failure("Билет не принадлежит пользователю", 403);
            }

            if (ticket.status == TicketStatus.Confirmed)
            {
                _logger.LogWarning("Попытка оплаты уже оплаченного билета {TicketId}", request.TicketId);
                return Result<PaymentDto>.Failure("Билет уже оплачен", 409);
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                TicketId = request.TicketId,
                UserId = request.UserId,
                Price = ticket.Price,
                Status = PaymentStatus.Pending,
                PaymentCreationDate = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment, ct);
            await _paymentRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Платёж {PaymentId} создан для билета {TicketId}", payment.Id, request.TicketId);

            return Result<PaymentDto>.Success(new PaymentDto
            {
                Id = payment.Id,
                TicketId = payment.TicketId,
                UserId = payment.UserId,
                Price = payment.Price,
                Status = payment.Status,
                PaymentCreationDate = payment.PaymentCreationDate,
                PayUrl = $"/pay/{payment.Id}"
            });
        }
    }
}
