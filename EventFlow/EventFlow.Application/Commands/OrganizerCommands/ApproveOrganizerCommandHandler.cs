using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.OrganizerCommands
{
    public class ApproveOrganizerCommandHandler : IRequestHandler<ApproveOrganizerCommand, Result>
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ApproveOrganizerCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveOrganizerCommandHandler(
            IOrganizerRepository organizerRepository, IUserRepository userRepository, ILogger<ApproveOrganizerCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _organizerRepository = organizerRepository;
            _userRepository = userRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ApproveOrganizerCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Утверждение заявки организатора с ID: {Id}", request.Id);
            
            var organizer = await _organizerRepository.GetByIdAsync(request.Id, ct);
            if (organizer == null)
            {
                _logger.LogWarning("Заявка организатора с ID {Id} не найдена", request.Id);
                return Result.Failure("Заявка организатора не найдена", 404);
            }
            
            if (organizer.IsVerified)
            {
                _logger.LogWarning("Заявка организатора с ID {Id} уже была утверждена", request.Id);
                return Result.Failure("Заявка уже была утверждена", 409);
            }
            
            var user = await _userRepository.GetByIdAsync(organizer.UserId);
            if(user is null)
            {
                _logger.LogWarning("пользователь {Id} не найден", organizer.UserId);
                return Result.Failure($"Пользователь с Id {organizer.UserId} не найден", 404);
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync(ct);
                
                organizer.IsVerified = true;
                int affected = await _organizerRepository.SaveChangesAsync(ct);
                if (affected == 0)
                {
                    throw new Exception("Не удалось сохранить статус организатора");
                }
                
                bool roleResult = await _userRepository.IsInRoleAsync(user, "organizer");
                if (!roleResult)
                {
                    var result = await _userRepository.AddNewRole(user, "organizer");
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Не удалось назначить роль организатора пользователю {user.Id}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                
                await _unitOfWork.CommitAsync(ct);
                _logger.LogInformation("Заявка организатора с ID {Id} успешно утверждена", request.Id);

                return Result.Success(200, "Заявка подтверждена");
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackAsync(ct);
                }
                catch(Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Не удалось откатить транзакцию для заявки {Id}", request.Id);
                }
                _logger.LogError(ex, "Произошла ошибка при изменении роли по заявке {id}", request.Id);
                return Result.Failure("Ошибка при изменении роли по заявке", 400);
            }
        }
    }
}