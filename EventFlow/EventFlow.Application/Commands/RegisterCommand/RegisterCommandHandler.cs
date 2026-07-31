using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Commands.RegisterCommand
{
    public class RegisterCommandHandler : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IRepository<User> _repository;
        private readonly IValidator<RegisterUserCommand> _validator;
        public RegisterCommandHandler(IRepository<User> repository, IValidator<RegisterUserCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }
        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken ct)
        {//В методах нужно будет дописать в параметры токен, как только изменят методы
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result.Failure(validationResult.Errors.First().ErrorMessage);
            //if (await _repository.GetByEmailAsync(request.Email) != null)
            //    return Result.Failure("Email занят");
            var user = new User();
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            await _repository.AddAsync(user, ct);
            await _repository.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}