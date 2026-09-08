using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.RegisterCommand
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegisterUserCommand> _validator;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        public RegisterUserCommandHandler(IUserRepository userRepository,
                                          IValidator<RegisterUserCommand> validator,
                                          ITokenService tokenService,
                                          ILogger<RegisterUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _validator = validator;
            _tokenService = tokenService;
            _logger = logger;
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
                  "Пользователь с таким {Email} уже зарегистрирован: {Error}",
                  request.Email,
                  validationResult.Errors.First().ErrorMessage);
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

            var result = await _userRepository.AddAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(',', result.Errors.Select(e => $"{e.Code}: {e.Description}"));

                _logger.LogWarning(
                    "Ошибка при регистрации для {Email}: {Error}",
                    request.Email,
                    errors);

                return Result<AuthResponseDto>.Failure(errors, 400);
            }
            return Result.Success();
        }
    }
}