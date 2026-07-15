using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
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
            //тут обновление моделей должно быть
            await _userRepository.SaveChangesAsync();
            return Result.Success();
        }
    }
}