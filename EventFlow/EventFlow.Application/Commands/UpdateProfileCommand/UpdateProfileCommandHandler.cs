using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Commands.UpdateProfileCommand
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UpdateProfileCommand> _validator;

        public UpdateProfileCommandHandler(IUserRepository userRepository, IValidator<UpdateProfileCommand> validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken ct)
        {//В методах нужно будет дописать в параметры токен, как только изменят методы
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result.Failure(validationResult.Errors.First().ErrorMessage);
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return Result.Failure("Пользователь не найден");
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            if (await _userRepository.ExistsByEmailAsync(user.Email))
            {
                return Result.Failure("Такой Email уже существует");
            }
            var result = await _userRepository.Update(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(',', result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                return Result.Failure(errors);
            }
            return Result.Success();
        }
    }
}