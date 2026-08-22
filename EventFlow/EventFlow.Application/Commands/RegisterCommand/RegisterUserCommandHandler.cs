using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventFlow.Application.Commands.RegisterCommand
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegisterUserCommand> _validator;
        private readonly ITokenService _tokenService;

        public RegisterUserCommandHandler(IUserRepository userRepository, IValidator<RegisterUserCommand> validator, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _validator = validator;
            _tokenService = tokenService;
        }
        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken ct)
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
            return Result.Success();
        }
    }
}