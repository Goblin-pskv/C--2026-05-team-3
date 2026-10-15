using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.RegisterCommand
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RegisterUserCommand> _validator;
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly IEmailConfirmTokenService _emailConfirmTokenService;
        private readonly IMessageService _emailService;

        public RegisterUserCommandHandler(IUserRepository userRepository,
                                          IUnitOfWork unitOfWork,
                                          IValidator<RegisterUserCommand> validator,
                                          ILogger<RegisterUserCommandHandler> logger,
                                          IEmailConfirmTokenService emailConfirmTokenService,
                                          IMessageService messageService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
            _emailConfirmTokenService = emailConfirmTokenService;
            _emailService = messageService;
        }
        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            _logger.LogInformation(
               "Регистрация начата для {Email} ({UserName})",
               request.Email,
               request.UserName
           );
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                   "Валидация не пройдена для {Email}: {Error}",
                   request.Email,
                   validationResult.Errors.First().ErrorMessage);

                return Result<AuthResponseDto>.Failure(validationResult.Errors.First().ErrorMessage, 422);
            }


            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                _logger.LogWarning(
                  "Пользователь с таким {Email} уже зарегистрирован.",
                  request.Email);
                return Result<AuthResponseDto>.Failure("Пользователь с таким Email уже зарегистрирован", 409);
            }
            var user = new User
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };

            try
            {
                await _unitOfWork.BeginTransactionAsync(ct);
                var addUserResult = await _userRepository.AddAsync(user,request.Password);
                if(!addUserResult.Succeeded)
                {
                    throw new Exception("Не удалось добавить нового пользователя");
                }
                var addRole = await _userRepository.AddNewRole(user, "user");
                if(!addRole.Succeeded)
                {
                    throw new Exception("Не удалось добавить роль для нового пользователя");
                }
                await _unitOfWork.CommitAsync(ct);
            }
            catch(Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackAsync(ct);
                }
                catch(Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Не удалось откатить транзакцию при регистрации нового пользователя");
                }
                _logger.LogError(ex, "Произошла ошибка при регистрации нового пользователя для {email}", request.Email);
                return Result.Failure("Ошибка при регистрации для ", 400);
            }
            _logger.LogInformation("Пользователь с email:{email} успешно создан", request.Email);
            try
            {
                var token = await _emailConfirmTokenService.GenerateAsync(user.Id, ct);
                await _emailService.SendEmailAsync(user.Email, "Подтверждение регистрации на EventFlow",
                    $@"Регистрация успешно завершена. Перейдите по ссылке, чтобы подтвердить свою регистрацию:{request.urlToController}?userId={Uri.EscapeDataString(user.Id.ToString())}&token={Uri.EscapeDataString(token)}", ct);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Пользователь {Email} создан, но письмо не отправлено", request.Email);
            }
            return Result.Success(200, "Пользователь создан");
        }
    }
}