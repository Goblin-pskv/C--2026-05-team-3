using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.AuthCommands
{
    public class RefreshUserCommandHandler : IRequestHandler<RefreshUserCommand, Result<AuthResponseDto>>
    {
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenHasher _tokenHasher;
        private readonly ILogger<RefreshUserCommand> _logger;

        public RefreshUserCommandHandler(ITokenService tokenService, IRefreshTokenService refreshTokenService, ITokenHasher tokenHasher, ILogger<RefreshUserCommand> logger)
        {
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _tokenHasher = tokenHasher;
            _logger = logger;
        }
        public async Task<Result<AuthResponseDto>> Handle(RefreshUserCommand request, CancellationToken cancellationToken)
        {
            string refreshToken = _tokenHasher.Hash(request.refresh);
            _logger.LogInformation("Обновление токена начато для refresh:{refresh}", refreshToken);
            User? user = await _refreshTokenService.ValidateRefreshTokenAsync(refreshToken);
            if (user is null)
            {
                _logger.LogInformation("не найден действующий refresh:{refresh}",refreshToken);
                return Result<AuthResponseDto>.Failure("Не верный токен", 401);
            }
            var (accessToken, expiresAt) = await _tokenService.GenerateTokenAsync(user!);
            _logger.LogInformation("Сгенерирован новый токен для:{refresh}", refreshToken);
            var response = new AuthResponseDto(accessToken, request.refresh, expiresAt);
            return Result<AuthResponseDto>.Success(response);
        }
    }
}