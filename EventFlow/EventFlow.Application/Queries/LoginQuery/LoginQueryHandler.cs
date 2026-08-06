using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Queries.LoginQuery
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<LoginQuery> _validator;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshiTokenService;

        public LoginQueryHandler(IUserRepository userRepository, IValidator<LoginQuery> validator, ITokenService tokenService, IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _validator = validator;
            _tokenService = tokenService;
            _refreshiTokenService = refreshTokenService;
        }
        public async Task<Result<AuthResponseDto>> Handle(LoginQuery request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result<AuthResponseDto>.Failure(validationResult.Errors.First().ErrorMessage);
            User? user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Result<AuthResponseDto>.Failure("Пользователь с таким Email и пароль не найден");
            if (!await _userRepository.CheckPasswordAsync(user, request.Password))
                return Result<AuthResponseDto>.Failure("Пользователь с таким Email и пароль не найден");
            string accessToken = await _tokenService.GenerateTokenAsync(user);
            string refreshToken = "";//await _refreshiTokenService.GenerateAndSaveRefreshTokenAsync(user.Id);
            var response = new AuthResponseDto(accessToken, refreshToken, DateTime.UtcNow);
            return Result<AuthResponseDto>.Success(response);
        }
    }
}