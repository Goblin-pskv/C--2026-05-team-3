using EventFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    /// <summary>
    /// Контракт для работы с регистрациями на мероприятия.
    /// 
    /// Registration — связь между User и Event (кто записался на что).
    /// Уникальное ограничение: один User может иметь одну Registration на один Event.
    /// 
    /// Особенности:
    /// - Проверка дубликатов (один пользователь = одна регистрация)
    /// - Подсчет количества участников
    /// - Получение списка участников мероприятия
    /// </summary>
    public interface IRegistrationRepository
    {
        /// <summary>
        /// Получить регистрацию по ID.
        /// </summary>
        /// <param name="id">UUID регистрации</param>
        /// <returns>Регистрация или null</returns>
        Task<Registration?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить все регистрации на конкретное мероприятие.
        /// Используется организатором для просмотра списка участников.
        /// </summary>
        /// <param name="eventId">UUID мероприятия</param>
        /// <returns>Список регистраций на мероприятие</returns>
        Task<List<Registration>> GetByEventIdAsync(Guid eventId);

        /// <summary>
        /// Получить все регистрации конкретного пользователя.
        /// Используется в личном кабинете для просмотра истории.
        /// </summary>
        /// <param name="userId">UUID пользователя</param>
        /// <returns>Список регистраций пользователя</returns>
        Task<List<Registration>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Проверить, зарегистрирован ли пользователь на мероприятие.
        /// Используется перед регистрацией для предотвращения дубликатов.
        /// </summary>
        /// <param name="eventId">UUID мероприятия</param>
        /// <param name="userId">UUID пользователя</param>
        /// <returns>true если регистрация существует</returns>
        /// <example>
        /// if (await _repository.ExistsAsync(eventId, userId))
        ///     throw new DomainException("Вы уже зарегистрированы");
        /// </example>
        Task<bool> ExistsAsync(Guid eventId, Guid userId);

        /// <summary>
        /// Получить количество подтвержденных регистраций на мероприятие.
        /// Используется для проверки доступности мест.
        /// </summary>
        /// <param name="eventId">UUID мероприятия</param>
        /// <returns>Количество подтвержденных регистраций</returns>
        Task<int> GetConfirmedCountAsync(Guid eventId);

        /// <summary>
        /// Добавить новую регистрацию.
        /// </summary>
        /// <param name="registration">Регистрация для добавления</param>
        Task AddAsync(Registration registration);

        /// <summary>
        /// Обновить статус регистрации.
        /// </summary>
        /// <param name="registration">Регистрация с измененным статусом</param>
        void Update(Registration registration);

        /// <summary>
        /// Удалить регистрацию.
        /// </summary>
        /// <param name="registration">Регистрация для удаления</param>
        void Delete(Registration registration);

        /// <summary>
        /// Сохранить все изменения в базе данных.
        /// </summary>
        Task SaveChangesAsync();
    }
}
