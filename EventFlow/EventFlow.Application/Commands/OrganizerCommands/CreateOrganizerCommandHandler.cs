using EventFlow.Application.Common;
using EventFlow.Application.Common.Validators;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EventFlow.Application.Commands.OrganizerCommands
{
    public class CreateOrganizerCommandHandler : IRequestHandler<CreateOrganizerCommand, Result>
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly ILogger<CreateOrganizerCommandHandler> _logger;
        private readonly IValidator<CreateOrganizerCommand> _validator;
        private readonly IUserRepository _userRepository;
        private readonly IMessageService _emailService;

        public CreateOrganizerCommandHandler(IOrganizerRepository organizerRepository, 
                                             ILogger<CreateOrganizerCommandHandler> logger, 
                                             IValidator<CreateOrganizerCommand> validator,
                                             IUserRepository userRepository,
                                             IMessageService emailService)
        {
            _organizerRepository = organizerRepository;
            _logger = logger;
            _validator = validator;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<Result> Handle(CreateOrganizerCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Создание новой заявки организатора");
            
            // Валидация запроса
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Валидация запроса не пройдена: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)), 400);
            }
            
            var userId = request.userId;
            
            // Проверяем, что пользователь существует
            var user = await _userRepository.GetByIdAsync((Guid)userId);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с ID {UserId} не найден", userId);
                return Result.Failure("Пользователь не найден", 404);
            }
            
            // Проверяем, что у пользователя нет уже заявки организатора
            var existingOrganizer = await _organizerRepository.GetByUserIdAsync((Guid)userId, ct);
            if (existingOrganizer != null)
            {
                if (existingOrganizer.IsVerified)
                {
                    _logger.LogWarning("Пользователь с ID {UserId} уже является организатором", userId);
                    return Result.Failure("Вы уже являетесь организатором", 409);
                }
                else if(!existingOrganizer.IsVerified && !existingOrganizer.IsDelete)
                {
                    _logger.LogWarning("Пользователь с ID {UserId} уже подал заявку на регистрацию", userId);
                    return Result.Failure("Вы уже подали заявку на регистрацию", 409);
                }
            }

            if (existingOrganizer is null)
            {
                // Создаем новую заявку организатора
                var organizer = new Organizer
                {
                    UserId = (Guid)userId!,
                    CompanyName = request.CompanyName,
                    Description = request.Description,
                    WebSite = request.WebSite,
                    IsVerified = false
                };
                await _organizerRepository.AddAsync(organizer, ct);
            }
            else
            {
                existingOrganizer.CompanyName = request.CompanyName;
                existingOrganizer.Description = request.Description;
                existingOrganizer.WebSite = request.WebSite;
                existingOrganizer.IsDelete = false;
                await _organizerRepository.UpdateAsync(existingOrganizer, ct);
            }
            await _organizerRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Заявка организатора для пользователя {UserId} успешно создана", userId);

            // Получаем информацию о пользователе для отправки email
            if (user != null)
            {
                try
                {
                    // Отправляем email пользователю с подтверждением о подаче заявки
                    await _emailService.SendEmailAsync(user.Email!, "Заявка на регистрацию профиля организатора", 
                        $@"Здравствуйте, {user.FirstName} {user.LastName}!

Ваша заявка на регистрацию профиля организатора успешно подана. 
Мы рассмотрим вашу заявку в ближайшее время и уведомим вас о результате.

С уважением,
Команда EventFlow", ct);
                    _logger.LogInformation("Email с подтверждением о подаче заявки отправлен пользователю {Email}", user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при отправке email пользователю {Email} после создания заявки организатора", user.Email);
                    // Продолжаем выполнение, так как ошибка отправки email не должна привести к отмене операции
                }
            }

            return Result.Success(200, "Заявка на организатора успешно подана");
        }
    }
}