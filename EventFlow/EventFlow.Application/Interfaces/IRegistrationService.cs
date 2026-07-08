using EventFlow.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    /// <summary>
    /// Контракт для бизнес-логики работы с регистрациями на мероприятия.
    /// 
    /// Отвечает за:
    /// - Регистрацию пользователей на мероприятия
    /// - Отмену регистраций
    /// - Подтверждение регистраций (организатором)
    /// - Отметку присутствия
    /// 
    /// Бизнес-правила, которые проверяет сервис:
    /// 1. Мероприятие должно быть опубликовано
    /// 2. Должны быть свободные места
    /// 3. Пользователь не должен быть уже зарегистрирован
    /// 4. Регистрация должна быть в правильном статусе для операции
    /// </summary>
    public interface IRegistrationService
    {
        /// <summary>
        /// Зарегистрировать пользователя на мероприятие.
        /// Проверяет наличие мест и отсутствие дубликатов.
        /// </summary>
        /// <param name="eventId">ID мероприятия</param>
        /// <param name="userId">ID пользователя из JWT токена</param>
        /// <returns>DTO созданной регистрации</returns>
        /// <exception cref="DomainException">Если нет мест или уже зарегистрирован</exception>
        Task<RegistrationDto> RegisterForEventAsync(Guid eventId, Guid userId);

        /// <summary>
        /// Отменить регистрацию пользователя.
        /// Освобождает место для других участников.
        /// </summary>
        /// <param name="eventId">ID мероприятия</param>
        /// <param name="userId">ID пользователя</param>
        Task CancelRegistrationAsync(Guid eventId, Guid userId);

        /// <summary>
        /// Подтвердить регистрацию (для организатора).
        /// Переводит статус из Pending в Confirmed.
        /// </summary>
        /// <param name="registrationId">ID регистрации</param>
        /// <param name="organizerId">ID организатора (проверка прав)</param>
        Task ConfirmRegistrationAsync(Guid registrationId, Guid organizerId);

        /// <summary>
        /// Отметить присутствие пользователя на мероприятии.
        /// Вызывается организатором во время мероприятия.
        /// </summary>
        /// <param name="registrationId">ID регистрации</param>
        /// <param name="organizerId">ID организатора</param>
        Task MarkAsAttendedAsync(Guid registrationId, Guid organizerId);

        /// <summary>
        /// Получить все регистрации на мероприятие.
        /// Используется организатором для просмотра списка участников.
        /// </summary>
        /// <param name="eventId">ID мероприятия</param>
        /// <returns>Список DTO регистраций</returns>
        Task<List<RegistrationDto>> GetRegistrationsByEventAsync(Guid eventId);

        /// <summary>
        /// Получить все регистрации пользователя.
        /// Используется в личном кабинете.
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список DTO регистраций</returns>
        Task<List<RegistrationDto>> GetRegistrationsByUserAsync(Guid userId);
    }
}
