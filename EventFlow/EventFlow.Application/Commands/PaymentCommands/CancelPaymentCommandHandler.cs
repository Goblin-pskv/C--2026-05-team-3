using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.PaymentCommands
{
    public class CancelPaymentCommandHandler : IRequestHandler<CancelPaymentCommand, Result<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CancelPaymentCommandHandler> _logger;

        public CancelPaymentCommandHandler(
            IPaymentRepository paymentRepository,
            ITicketRepository ticketRepository,
            IRegistrationRepository registrationRepository,
            IUnitOfWork unitOfWork,
            ILogger<CancelPaymentCommandHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _ticketRepository = ticketRepository;
            _registrationRepository = registrationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<PaymentDto>> Handle(CancelPaymentCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Отмена платежа {Id} пользователем {UserId}", request.Id, request.UserId);

            var payment = await _paymentRepository.GetByIdWithIncludesAsync(request.Id, ct);
            if (payment == null)
            {
                _logger.LogWarning("Платёж {Id} не найден", request.Id);
                return Result<PaymentDto>.Failure("Платёж не найден", 404);
            }

            if (payment.UserId != request.UserId)
            {
                _logger.LogWarning("Попытка отменить чужой платёж: платёж {Id}, пользователь {UserId}", request.Id, request.UserId);
                return Result<PaymentDto>.Failure("Платёж не принадлежит пользователю", 403);
            }

            if (payment.Status is PaymentStatus.Confirmed or PaymentStatus.Returned)
            {
                _logger.LogWarning("Нельзя отменить платёж {Id} в статусе {Status}", request.Id, payment.Status);
                return Result<PaymentDto>.Failure($"Нельзя отменить платёж в статусе {payment.Status}", 409);
            }

            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                payment.Status = PaymentStatus.Cancelled;
                payment.LastModificationDate = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment, ct);

                var ticket = await _ticketRepository.GetByIdWithIncludesAsync(payment.TicketId, ct);
                if (ticket != null)
                {
                    ticket.status = TicketStatus.Cancelled;
                    ticket.LastModificationDate = DateTime.UtcNow;
                    await _ticketRepository.UpdateAsync(ticket, ct);
                }

                var registration = await _registrationRepository.GetRegistrationAsync(payment.UserId, ticket!.EventId, ct);
                if (registration != null && registration.Status != RegistrationStatus.Cancelled)
                {
                    registration.Cancel();
                    await _registrationRepository.UpdateAsync(registration, ct);
                }

                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitAsync(ct);

                _logger.LogInformation("Платёж {Id} переведён в статус Cancelled", payment.Id);

                return Result<PaymentDto>.Success(new PaymentDto
                {
                    Id = payment.Id,
                    TicketId = payment.TicketId,
                    UserId = payment.UserId,
                    Price = payment.Price,
                    Status = EnumHelper.GetDescription(PaymentStatus.Cancelled),
                    PaymentCreationDate = payment.PaymentCreationDate,
                    LastModificationDate = payment.LastModificationDate
                });
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}
