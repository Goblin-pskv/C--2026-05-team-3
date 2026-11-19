using EventFlow.Application.Common;
using EventFlow.Application.Common.Validators;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.OrganizerCommands
{
    public class CreateOrganizerRequestCommandHandler : IRequestHandler<CreateOrganizerRequestCommand, Result>
    {
        private readonly IOrganizerRequestRepository _organizerRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageService _emailService;
        private readonly ILogger<CreateOrganizerRequestCommandHandler> _logger;
        private readonly IValidator<CreateOrganizerRequestCommand> _validator;

        public CreateOrganizerRequestCommandHandler(
            IOrganizerRequestRepository organizerRequestRepository,
            IUserRepository userRepository,
            IMessageService emailService,
            ILogger<CreateOrganizerRequestCommandHandler> logger,
            IValidator<CreateOrganizerRequestCommand> validator)
        {
            _organizerRequestRepository = organizerRequestRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result> Handle(CreateOrganizerRequestCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Создание заявки на роль организатора (без создания профиля)");

            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Валидация запроса не пройдена: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)), 400);
            }

            var userId = request.userId;

            var user = await _userRepository.GetByIdAsync((Guid)userId);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с ID {UserId} не найден", userId);
                return Result.Failure("Пользователь не найден", 404);
            }

            var existingRequest = await _organizerRequestRepository.GetActiveByUserIdAsync((Guid)userId, ct);
            if (existingRequest != null)
            {
                _logger.LogWarning("Пользователь с ID {UserId} уже имеет активную заявку", userId);
                return Result.Failure("У вас уже есть активная заявка", 409);
            }

            var requestData = new { CompanyName = request.CompanyName, Description = request.Description, WebSite = request.WebSite };
            var dataJson = System.Text.Json.JsonSerializer.Serialize(requestData);

            var organizerRequest = new OrganizerRequest
            {
                UserId = (Guid)userId!,
                Status = Domain.Enums.OrganizerRequestStatus.Pending,
                Data = dataJson
            };
            await _organizerRequestRepository.AddAsync(organizerRequest, ct);
            await _organizerRequestRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Заявка на роль организатора для пользователя {UserId} успешно создана", userId);

            try
            {
                await _emailService.SendEmailAsync(user.Email!, "Заявка на роль организатора",
                    $@"Здравствуйте, {user.FirstName} {user.LastName}!

Ваша заявка на роль организатора успешно подана.
Мы рассмотрим вашу заявку в ближайшее время и уведомим вас о результате.

С уважением,
Команда EventFlow", ct);
                _logger.LogInformation("Email отправлен пользователю {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке email пользователю {Email}", user.Email);
            }

            return Result.Success(200, "Заявка на роль организатора успешно подана");
        }
    }
}
