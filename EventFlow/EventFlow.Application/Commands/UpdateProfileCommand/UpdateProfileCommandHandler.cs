using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Commands.UpdateProfileCommand
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
    {
        private readonly IRepository<User> _repository;
        private readonly IValidator<UpdateProfileCommand> _validator;

        public UpdateProfileCommandHandler(IRepository<User> repository, IValidator<UpdateProfileCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken ct)
        {//В методах нужно будет дописать в параметры токен, как только изменят методы
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result.Failure(validationResult.Errors.First().ErrorMessage);
            var user = await _repository.GetByIdAsync(request.UserId, ct);
            if (user == null)
                return Result.Failure("Пользователь не найден");
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            await _repository.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}