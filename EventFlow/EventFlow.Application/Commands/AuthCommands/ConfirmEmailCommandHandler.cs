using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFlow.Application.Commands.AuthCommands
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
    {
        private readonly IEmailConfirmTokenService _emailConfirmTokenService;
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;

        public ConfirmEmailCommandHandler(
            IEmailConfirmTokenService emailConfirmTokenService,
            ILogger<ConfirmEmailCommandHandler> logger)
        {
            _emailConfirmTokenService = emailConfirmTokenService;
            _logger = logger;
        }

        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Подтверждение email для пользователя {UserId}", request.userId);
                
                var isConfirmed = await _emailConfirmTokenService.ConfirmEmail(request.userId, request.token);
                
                if (!isConfirmed)
                {
                    _logger.LogWarning("Не удалось подтвердить email для пользователя {UserId}", request.userId);
                    return Result.Failure("Неверная ссылка подтверждения или срок действия токена истек", 400);
                }

                _logger.LogInformation("Email успешно подтвержден для пользователя {UserId}", request.userId);
                return Result.Success(200, "Email успешно подтвержден");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при подтверждении email для пользователя {UserId}", request.userId);
                return Result.Failure("Произошла ошибка при подтверждении email", 500);
            }
        }
    }
}