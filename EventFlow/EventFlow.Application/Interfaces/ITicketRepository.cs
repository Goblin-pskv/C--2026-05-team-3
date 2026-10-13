using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EventFlow.Application.Interfaces
{
    /// <summary>
    /// Контракт для работы с билетами.
    ///
    /// Ticket — связь между User и Event (кто записался на что),
    /// а также агрегат для платежей (Payments).
    ///
    /// Особенности:
    /// - Уникальное ограничение: один User = один Ticket на Event
    /// - У Ticket может быть много Payment (попытки, возвраты)
    /// - Поиск по QR-коду для check-in на мероприятии
    /// - Поиск по RegistrationId (связь с legacy-регистрацией)
    /// </summary>
    public interface ITicketRepository : IRepository<Tickets>
    {
        /// <summary>
        /// Получить все билеты пользователя.
        /// </summary>
        Task<List<Tickets>> GetByUserAsync(Guid userId, CancellationToken ct);

        /// <summary>
        /// Получить все билеты на мероприятие.
        /// </summary>
        Task<List<Tickets>> GetByEventAsync(Guid eventId, CancellationToken ct);

        ///// <summary>
        ///// Найти билет по QR-коду (для check-in).
        ///// </summary>
        //Task<Tickets> GetByQrCodeAsync(string code, CancellationToken ct);
               
        /// <summary>
        /// Найти билет по уникальному номеру.
        /// Например: "EVT-2026-000123".
        /// </summary>
        Task<Tickets> GetByNumberAsync(string ticketNumber, CancellationToken ct);

        /// <summary>
        /// Проверить, есть ли у пользователя билет на мероприятие.
        /// Используется для валидации уникальности (один User = один Ticket на Event).
        /// </summary>
        Task<bool> ExistsAsync(Guid userId, Guid eventId, CancellationToken ct);

        /// <summary>
        /// Получить билеты по статусу на мероприятие (например, только Success).
        /// </summary>
        Task<List<Tickets>> GetByStatusAsync(Guid eventId, TicketStatus status, CancellationToken ct);

        /// <summary>
        /// Считает количество подтверждённых билетов на мероприятие.
        /// Аналог CountConfirmedAsync в IRegistrationRepository.
        /// Используется для проверки лимита MaxParticipants.
        /// </summary>
        Task<int> CountConfirmedAsync(Guid eventId, CancellationToken ct);
    }
}