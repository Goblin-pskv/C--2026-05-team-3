using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.OrganizerCommands
{
    public class ApproveOrganizerCommandHandler : IRequestHandler<ApproveOrganizerCommand, Result>
    {
        private readonly IOrganizerRequestRepository _organizerRequestRepository;
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ApproveOrganizerCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveOrganizerCommandHandler(
            IOrganizerRequestRepository organizerRequestRepository,
            IOrganizerRepository organizerRepository,
            IUserRepository userRepository,
            ILogger<ApproveOrganizerCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _organizerRequestRepository = organizerRequestRepository;
            _organizerRepository = organizerRepository;
            _userRepository = userRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ApproveOrganizerCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Утверждение заявки организатора с ID: {Id}", request.Id);

            var organizerRequest = await _organizerRequestRepository.GetByIdAsync(request.Id, ct);
            if (organizerRequest == null)
            {
                _logger.LogWarning("Заявка организатора с ID {Id} не найдена", request.Id);
                return Result.Failure("Заявка организатора не найдена", 404);
            }

            if (organizerRequest.Status != Domain.Enums.OrganizerRequestStatus.Pending)
            {
                _logger.LogWarning("Заявка организатора с ID {Id} уже была рассмотрена", request.Id);
                return Result.Failure("Заявка уже была рассмотрена", 409);
            }

            var user = await _userRepository.GetByIdAsync(organizerRequest.UserId);
            if (user is null)
            {
                _logger.LogWarning("Пользователь {Id} не найден", organizerRequest.UserId);
                return Result.Failure($"Пользователь с Id {organizerRequest.UserId} не найден", 404);
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync(ct);

                organizerRequest.Status = Domain.Enums.OrganizerRequestStatus.Approved;
                organizerRequest.ReviewedById = request.AdminId;
                organizerRequest.ReviewedAt = DateTime.UtcNow;
                int affected = await _organizerRequestRepository.SaveChangesAsync(ct);
                if (affected == 0)
                {
                    throw new Exception("Не удалось сохранить статус заявки");
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

                // Создаём профиль организатора из данных заявки
                if (!string.IsNullOrEmpty(organizerRequest.Data))
                {
                    var requestData = System.Text.Json.JsonSerializer.Deserialize<OrganizerDataDto>(organizerRequest.Data);
                    if (requestData != null)
                    {
                        var organizer = new Organizer
                        {
                            UserId = organizerRequest.UserId,
                            CompanyName = requestData.CompanyName,
                            Description = requestData.Description,
                            WebSite = requestData.WebSite
                        };
                        await _organizerRepository.AddAsync(organizer, ct);
                        await _organizerRepository.SaveChangesAsync(ct);
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

        private record OrganizerDataDto(string? CompanyName, string? Description, string? WebSite);
    }
}