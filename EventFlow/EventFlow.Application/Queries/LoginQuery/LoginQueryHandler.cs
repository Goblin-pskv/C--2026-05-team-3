using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Queries.LoginQuery
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<string>>
    {
        private readonly IRepository<User> _repository;
        private readonly IValidator<LoginQuery> _validator;

        public LoginQueryHandler(IRepository<User> repository, IValidator<LoginQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }
        public async Task<Result<string>> Handle(LoginQuery request, CancellationToken ct)
        {//В методах нужно будет дописать в параметры токен, как только изменят методы
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result<string>.Failure(validationResult.Errors.First().ErrorMessage);
            //var user = await _repository.GetByEmailAsync(request.Email);
            //if (user == null/* || !_passwordHasher.Verify(request.Password, user.PasswordHash)*/)
            //    return Result<string>.Failure("Неверный Email или пароль");
            // замена на JWT позже. Нужно будет дописать проверку на пароль, когда появится hasher.
            var token = "fake-jwt-token";
            return Result<string>.Success(token);
        }
    }
}