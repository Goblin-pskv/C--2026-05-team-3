using EventFlow.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.API.Controllers
{
    /// <summary>
    /// Базовый контроллер для всех API контроллеров
    /// </summary>
    public class BaseController : ControllerBase
    {
        protected readonly IMediator _mediator;

        public BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Обработка результатов выполнения команды
        /// </summary>
        /// <param name="result">Результат выполнения команды</param>
        /// <returns>Ответ HTTP</returns>
        protected IActionResult HandleResult(Result? result)
        {
            if (result == null)
                return BadRequest("Произошла ошибка при обработке запроса");

            if (!result.IsSuccess)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 404:
                        return NotFound(result.Message);
                    case 409:
                        return Conflict(result.Message);
                    case 422:
                        return UnprocessableEntity(result.Message);
                    case 401:
                        return Unauthorized(result.Message);
                    default:
                        return BadRequest(result.Message);
                }
            }

            return Ok(result.Message);
        }

        /// <summary>
        /// Обработка результатов выполнения команды с возвращением объекта
        /// </summary>
        /// <param name="result">Результат выполнения команды</param>
        /// <returns>Ответ HTTP</returns>
        protected IActionResult HandleResult<T>(Result<T>? result)
        {
            if (result == null)
                return BadRequest("Произошла ошибка при обработке запроса");

            if (!result.IsSuccess)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 404:
                        return NotFound(result.Message);
                    case 409:
                        return Conflict(result.Message);
                    case 422:
                        return UnprocessableEntity(result.Message);
                    case 401:
                        return Unauthorized(result.Message);
                    default:
                        return BadRequest(result.Message);
                }
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Получение ID пользователя из токена
        /// </summary>
        /// <returns>ID пользователя или null, если пользователь не авторизован</returns>
        protected Guid? GetUserId()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return null;

            if (!Guid.TryParse(userId, out Guid userGuid))
                return null;

            return userGuid;
        }

        /// <summary>
        /// Проверка авторизации пользователя
        /// </summary>
        /// <returns>true если пользователь авторизован, иначе false</returns>
        protected bool IsUserAuthorized()
        {
            return GetUserId() != null;
        }
    }
}