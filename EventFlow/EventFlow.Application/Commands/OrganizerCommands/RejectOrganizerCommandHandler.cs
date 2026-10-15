using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.OrganizerCommands
{
    public class RejectOrganizerCommandHandler : IRequestHandler<RejectOrganizerCommand, Result>
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RejectOrganizerCommandHandler> _logger;

        public RejectOrganizerCommandHandler(IOrganizerRepository organizerRepository, IUserRepository userRepository, ILogger<RejectOrganizerCommandHandler> logger)
        {
            _organizerRepository = organizerRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Result> Handle(RejectOrganizerCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Отклонение заявки организатора с ID: {Id}", request.Id);
            
            var organizer = await _organizerRepository.GetByIdAsync(request.Id, ct);
            if (organizer == null)
            {
                _logger.LogWarning("Заявка организатора с ID {Id} не найдена", request.Id);
                return Result.Failure("Заявка организатора не найдена", 404);
            }
            
            // Проверяем, что заявка еще не отклонена
            if (organizer.IsDelete)
            {
                _logger.LogWarning("Заявка организатора с ID {Id} уже была отклонена", request.Id);
                return Result.Failure("Заявка уже была отклонена", 409);
            }
            
            organizer.IsDelete = true;
            organizer.Reason = request.Reason;
            
            try
            {
                await _organizerRepository.SaveChangesAsync(ct);
                _logger.LogInformation("Заявка организатора с ID {Id} успешно отклонена и удалена", request.Id);
                return Result.Success(200,"Заявка организатора отклонена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отклонении заявки организатора с ID {Id}", request.Id);
                return Result.Failure("Ошибка при отклонении заявки организатора", 500);
            }
        }
    }
}