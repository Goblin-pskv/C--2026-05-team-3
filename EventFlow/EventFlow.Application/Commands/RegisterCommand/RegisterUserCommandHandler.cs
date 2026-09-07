using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.RegisterCommand {
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result> {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegisterUserCommand> _validator;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(IUserRepository userRepository, IValidator<RegisterUserCommand> validator, ITokenService tokenService, ILogger<RegisterUserCommandHandler> logger)
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
            if (!validationResult.IsValid) {
                _logger.LogWarning(
                    "Валидация не пройдена для {Email}: {Error}",
                    request.Email,
                    validationResult.Errors.First().ErrorMessage);

                return Result<AuthResponseDto>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var user = new User();
            user.UserName = request.UserName;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            //if(await _userRepository.ExistsByEmailAsync(user.Email))
            //{
            //    return Result<AuthResponseDto>.Failure("Такой Email уже существует");
            //}
            var result = await _userRepository.AddAsync(user, request.Password);
            if (!result.Succeeded) {
                var errors = string.Join(',', result.Errors.Select(e => $"{e.Code}: {e.Description}"));

                _logger.LogWarning(
                    "Ошибка при регистрации для {Email}: {Error}",
                    request.Email,
                    errors);

                return Result<AuthResponseDto>.Failure(errors);
            }
            _logger.LogInformation(
                "Регистрация успешно завершена для {Email} ({UserName})",
                request.Email,
                request.UserName
            );
            return Result.Success();
        }
    }
}
