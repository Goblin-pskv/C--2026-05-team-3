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
    public interface IRegistrationRepository : IRepository<Registration>
    {
        Task<Registration?> GetRegistrationAsync(Guid userId, Guid eventId, CancellationToken ct);
        Task<List<Registration>> GetRegistrationsByUserIdAsync(Guid userId, CancellationToken ct);
        Task<List<Registration>> GetRegistrationsByEventIdAsync(Guid eventId, CancellationToken ct);

        /// <summary>
        /// Считает количество подтверждённых регистраций на мероприятие.
        /// Используется для проверки лимита MaxParticipants.
        /// </summary>
        Task<int> CountConfirmedAsync(Guid eventId, CancellationToken ct);
    }
}
