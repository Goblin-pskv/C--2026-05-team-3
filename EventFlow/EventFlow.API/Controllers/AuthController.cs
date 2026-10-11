using EventFlow.Application.Commands.AuthCommands;
using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.DTOs;
using EventFlow.Application.Queries.GetProfileQuery;
using EventFlow.Application.Queries.LoginQuery;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EventFlow.API.Controllers
{
    /// <summary>
    /// При помощи MediatR реализуем команды из EventFlow.Application.Commands
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="command">Получает на вход json с полями</param>
        /// <returns>Возвращает сообщение с кодом ответа</returns>
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUserCommand([FromBody] RegisterUserCommand command)
        {
            string? url = Url.RouteUrl("ConfirmEmail", null, Request.Scheme);
            var commandWith = command with { urlToController = url };
            var result = await _mediator.Send(commandWith);

            if (!result.IsSuccess)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 409:
                        return Conflict(result.Message);
                    case 422:
                        return UnprocessableEntity(result.Message);
                }
            }
            return Ok(result.Message);
        }
        /// <summary>
        /// Авторизация по Email и паролю
        /// </summary>
        /// <param name="command">Получает на вход json с полями</param>
        /// <returns>Возвращает данные токена, либо сообщение об ошибке</returns>
        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLoginCommand([FromBody] LoginQuery command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                switch(result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 404:
                        return NotFound(result.Message);
                    case 422:
                        return UnprocessableEntity(result.Message);
                }

            return Ok(result.Value);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshUserCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                switch (result.StatusCode)
                {
                    case 401:
                        return Unauthorized(result.Message);
                }
            return Ok(result);
        }
        [HttpGet("confirm-email", Name = "ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, string token)
        {
            var command = new ConfirmEmailCommand(Guid.Parse(userId), token);
            var result = await _mediator.Send(command);
            switch (result.StatusCode)
            {
                case 400:
                    return NotFound(result.Message);
            }
            return Ok(result.Message);
        }
    }
}