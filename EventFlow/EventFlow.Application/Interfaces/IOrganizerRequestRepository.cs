using EventFlow.Domain.Entities;

namespace EventFlow.Application.Interfaces
{
    /// <summary>
    /// Контракт для работы с заявками на роль организатора.
    /// </summary>
    public interface IOrganizerRequestRepository : IRepository<OrganizerRequest>
    {
        /// <summary>
        /// Получить активную (Pending) заявку пользователя по его ID.
        /// Возвращает null, если активной заявки нет.
        /// </summary>
        Task<OrganizerRequest?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct);

        /// <summary>
        /// Получить все заявки со статусом Pending (для администратора).
        /// </summary>
        /// <param name="offset">Смещение</param>
        /// <param name="count">Количество</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Коллекция заявок Pending</returns>
        Task<IEnumerable<OrganizerRequest>> GetPendingAsync(int offset, int count, CancellationToken ct);

        /// <summary>
        /// Получить общее количество заявок со статусом Pending.
        /// </summary>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Общее количество заявок Pending</returns>
        Task<int> GetPendingCountAsync(CancellationToken ct);
    }
}
