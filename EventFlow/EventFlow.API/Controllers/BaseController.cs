using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventFlow.API.Controllers
{

    /// <summary>
    /// Базовый контроллер для всех API endpoints.
    /// 
    /// Зачем нужен:
    /// 1. Общие атрибуты для всех контроллеров ([ApiController], [Route])
    /// 2. Вспомогательные методы для работы с пользователем (получение ID из JWT)
    /// 3. Стандартизированные ответы (успех, ошибка)
    /// 4. Избегаем дублирования кода в каждом контроллере
    /// 
    /// Все контроллеры наследуются от него:
    /// public class EventsController : BaseController { }
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Получить ID текущего пользователя из JWT токена.
        /// 
        /// Как работает:
        /// 1. JWT middleware извлекает токен из заголовка Authorization
        /// 2. Токен валидируется и claims добавляются в User
        /// 3. Мы берем claim с типом NameIdentifier (это ID пользователя)
        /// 
        /// Используется во всех методах, где нужен userId:
        /// - Создание мероприятия (organizerId)
        /// - Регистрация на мероприятие (userId)
        /// - Получение своих регистраций
        /// </summary>
        /// <returns>UUID текущего пользователя</returns>
        /// <exception cref="UnauthorizedAccessException">Если пользователь не аутентифицирован</exception>
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Пользователь не аутентифицирован");
           
            if (!Guid.TryParse(userIdClaim, out Guid parsedGuid))
            {
                throw new UnauthorizedAccessException("Идентификатор пользователя имеет неверный формат");
            }

            return parsedGuid;
        }
        /// <summary>
        /// Получить Email текущего пользователя из JWT токена.
        /// </summary>
        /// <returns>Email пользователя</returns>
        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Получить роль текущего пользователя из JWT токена.
        /// </summary>
        /// <returns>Роль (Participant или Organizer)</returns>
        protected string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        /// <summary>
        /// Проверить, является ли текущий пользователь организатором.
        /// </summary>
        /// <returns>true если роль Organizer</returns>
        protected bool IsOrganizer()
        {
            return User.IsInRole("Organizer");
        }

        /// <summary>
        /// Создать стандартный успешный ответ.
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <param name="data">Данные для возврата</param>
        /// <param name="message">Сообщение (опционально)</param>
        /// <returns>HTTP 200 с данными</returns>
        protected ActionResult Success<T>(T data, string? message = null)
        {
            return Ok(new
            {
                success = true,
                message = message ?? "Выполнено успешно", data
            });
        }

        /// <summary>
        /// Создать ответ об ошибке.
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="statusCode">HTTP статус код (по умолчанию 400)</param>
        /// <returns>HTTP ошибка с сообщением</returns>
        protected ActionResult Error(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, new
            {
                success = false,
                message
            });
        }
    }
}
