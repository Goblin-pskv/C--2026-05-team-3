using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.UpdateProfileCommand
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UpdateProfileCommand> _validator;
        private ILogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(IUserRepository userRepository,
                                           IValidator<UpdateProfileCommand> validator,
                                           ILogger<UpdateProfileCommandHandler> logger)
        {
            _userRepository = userRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken ct)
        {//В методах нужно будет дописать в параметры токен, как только изменят методы
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                 "Ошибка валидации для {Email}: {Error}",
                 request.Email,
                 validationResult.Errors.First().ErrorMessage);

                return Result.Failure(validationResult.Errors.First().ErrorMessage, 422);
            }
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning(
                    "Пользователь с Id {UserId} не найден",
                    request.UserId
                    );

                return Result.Failure("Пользователь не найден", 404);
            }
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            if (await _userRepository.ExistsByEmailAsync(user.Email))
            {
                return Result.Failure("Такой Email уже существует", 409);
            }
            var result = await _userRepository.Update(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "Обновление не удалось для {UserId}",
                    request.UserId);

                var errors = string.Join(',', result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                return Result.Failure(errors, 400);
            }

            _logger.LogInformation(
                "Данные пользователя с Id {UserId} успешно обновлены",
                request.UserId
                );

            return Result.Success();
        }
    }
}