using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Queries.LoginQuery
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<LoginQuery> _validator;

        public LoginQueryHandler(IUserRepository userRepository, IValidator<LoginQuery> validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }
        public async Task<Result<string>> Handle(LoginQuery request, CancellationToken ct)
        {//В методах нужно будет дописать в параметры токен, как только изменят методы
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result<string>.Failure(validationResult.Errors.First().ErrorMessage);
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null/* || !_passwordHasher.Verify(request.Password, user.PasswordHash)*/)
                return Result<string>.Failure("Неверный Email или пароль");
            // замена на JWT позже. Нужно будет дописать проверку на пароль, когда появится hasher.
            var token = "fake-jwt-token";
            return Result<string>.Success(token);
        }
    }
}