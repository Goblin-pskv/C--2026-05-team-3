using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Commands.RegisterCommand
{
    public class RegisterCommandHandler : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegisterUserCommand> _validator;
        public RegisterCommandHandler(IUserRepository userRepository, IValidator<RegisterUserCommand> validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }
        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken ct)
        {//В методах нужно будет дописать в параметры токен, как только изменят методы
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result.Failure(validationResult.Errors.First().ErrorMessage);
            if (await _userRepository.GetByEmailAsync(request.Email) != null)
                return Result.Failure("Email занят");
            var user = new User(request.FirstName, request.LastName, request.Email, request.PasswordHash);
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return Result.Success();
        }
    }
}