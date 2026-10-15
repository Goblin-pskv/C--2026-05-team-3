using EventFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    /// <summary>
    /// Контракт для работы с профилями организаторов в базе данных.
    /// 
    /// Organizer — это расширение User для тех, кто создает мероприятия.
    /// Один User может иметь один Organizer профиль (связь 1:1).
    /// 
    /// Когда нужен:
    /// - При создании мероприятия (проверка, что пользователь — организатор)
    /// - При отображении информации об организаторе на странице мероприятия
    /// - При верификации организатора администратором
    /// </summary>
    public interface IOrganizerRepository : IRepository<Organizer>
    {
        /// <summary>
        /// Получить профиль организатора по ID пользователя.
        /// Используется для проверки, является ли пользователь организатором.
        /// </summary>
        /// <param name="userId">UUID пользователя</param>
        /// <returns>Профиль организатора или null, если пользователь — участник</returns>
        /// <example>
        /// var organizer = await _repository.GetByUserIdAsync(userId);
        /// if (organizer == null)
        ///     throw new DomainException("Пользователь не является организатором");
        /// </example>
        Task<Organizer?> GetByUserIdAsync(Guid userId, CancellationToken ct);

        /// <summary>
        /// Получить все неверифицированные профили организаторов (для администратора).
        /// </summary>
        /// <returns>Список неверифицированных профилей организаторов</returns>
        Task<IEnumerable<Organizer>> GetPendingAsync(int offset, int count, CancellationToken ct);
    }
}
