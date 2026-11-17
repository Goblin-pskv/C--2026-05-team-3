using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.OrganizerCommands
{
    public class RejectOrganizerCommandHandler : IRequestHandler<RejectOrganizerCommand, Result>
    {
        private readonly IOrganizerRequestRepository _organizerRequestRepository;
        private readonly ILogger<RejectOrganizerCommandHandler> _logger;

        public RejectOrganizerCommandHandler(IOrganizerRequestRepository organizerRequestRepository, ILogger<RejectOrganizerCommandHandler> logger)
        {
            _organizerRequestRepository = organizerRequestRepository;
            _logger = logger;
        }

        public async Task<Result> Handle(RejectOrganizerCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Отклонение заявки организатора с ID: {Id}", request.Id);

            var organizerRequest = await _organizerRequestRepository.GetByIdAsync(request.Id, ct);
            if (organizerRequest == null)
            {
                _logger.LogWarning("Заявка организатора с ID {Id} не найдена", request.Id);
                return Result.Failure("Заявка организатора не найдена", 404);
            }

            // Проверяем, что заявка ещё не рассмотрена
            if (organizerRequest.Status != Domain.Enums.OrganizerRequestStatus.Pending)
            {
                _logger.LogWarning("Заявка организатора с ID {Id} уже была рассмотрена", request.Id);
                return Result.Failure("Заявка уже была рассмотрена", 409);
            }

            organizerRequest.Status = Domain.Enums.OrganizerRequestStatus.Rejected;
            organizerRequest.RejectionReason = request.Reason;
            organizerRequest.ReviewedById = request.AdminId;
            organizerRequest.ReviewedAt = DateTime.UtcNow;

            try
            {
                await _organizerRequestRepository.SaveChangesAsync(ct);
                _logger.LogInformation("Заявка организатора с ID {Id} успешно отклонена", request.Id);
                return Result.Success(200, "Заявка организатора отклонена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отклонении заявки организатора с ID {Id}", request.Id);
                return Result.Failure("Ошибка при отклонении заявки организатора", 500);
            }
        }
    }
}