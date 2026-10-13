using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventFlow.Application.Interfaces
{
    /// <summary>
    /// Контракт для работы с платежами.
    ///
    /// Payment — платёж по билету (Ticket).
    /// Связывает User (плательщик) и Ticket.
    ///
    /// У одного Ticket может быть много Payment:
    /// - первая попытка оплаты;
    /// - повторная попытка после ошибки;
    /// - возврат средств.
    ///
    /// Наследует базовые CRUD-операции из IRepository&lt;Payment&gt;.
    /// Дополнительно определяет поиск по пользователю и билету.
    /// </summary>
    public interface IPaymentRepository : IRepository<Payment>
    {
        /// <summary>
        /// Получить все платежи пользователя.
        /// </summary>
        Task<List<Payment>> GetByUserAsync(Guid userId, CancellationToken ct);

        /// <summary>
        /// Получить все платежи по билету.
        /// Включает попытки оплаты и возвраты.
        /// </summary>
        Task<List<Payment>> GetByTicketAsync(Guid ticketId, CancellationToken ct);

        /// <summary>
        /// Получить платежи по статусу (например, только Confirmed).
        /// </summary>
        Task<List<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken ct);

        /// <summary>
        /// Проверить, есть ли у билета успешный платёж.
        /// Используется для валидации подтверждения билета.
        /// </summary>
        Task<bool> HasConfirmedPaymentAsync(Guid ticketId, CancellationToken ct);

        /// <summary>
        /// Сумма всех успешных платежей по билету минус возвраты.
        /// </summary>
        Task<decimal> GetTotalPaidAsync(Guid ticketId, CancellationToken ct);
    }
}