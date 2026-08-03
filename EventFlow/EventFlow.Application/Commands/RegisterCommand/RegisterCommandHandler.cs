using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventFlow.Application.Commands.RegisterCommand
{
    public class RegisterCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegisterUserCommand> _validator;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshiTokenService;

        public RegisterCommandHandler(IUserRepository userRepository, IValidator<RegisterUserCommand> validator, ITokenService tokenService, IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _validator = validator;
            _tokenService = tokenService;
            _refreshiTokenService = refreshTokenService;
        }
        public async Task<Result<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result<AuthResponseDto>.Failure(validationResult.Errors.First().ErrorMessage);
            var user = new User();
            user.UserName = request.UserName;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            if(await _userRepository.ExistsByEmailAsync(user.Email))
            {
                return Result<AuthResponseDto>.Failure("Такой Email уже существует");
            }
            var result = await _userRepository.AddAsync(user, request.PasswordHash);
            if(!result.Succeeded)
            {
                var errors = string.Join(',', result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                return Result<AuthResponseDto>.Failure(errors);
            }
            string accessToken = "";//await _tokenService.GenerateTokenAsync(user);
            string refreshToken = "";//await _refreshiTokenService.GenerateAndSaveRefreshTokenAsync(user.Id);
            var response = new AuthResponseDto(accessToken, refreshToken, DateTime.UtcNow);
            return Result<AuthResponseDto>.Success(response);
        }
    }
}