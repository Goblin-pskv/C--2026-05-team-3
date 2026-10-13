using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using EventFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventFlow.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий платежей на EF Core.
    /// Наследует базовый CRUD из BaseRepository&lt;Payment&gt;.
    /// </summary>
    public class PaymentRepository(EventFlowDbContext context)
        : BaseRepository<Payment>(context), IPaymentRepository
    {

        /// <summary>
        /// Получить платёж по Id с подгрузкой Ticket и User.
        /// </summary>
        /// <param name="id">UUID платежа</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>Payment или null</returns>
        public override async Task<Payment?> GetByIdWithIncludesAsync(Guid id, CancellationToken ct)
        {
            return await _dbSet
                .Include(p => p.Ticket)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        /// <summary>
        /// Все платежи пользователя (с билетом),
        /// отсортированные по дате создания (новые сверху).
        /// </summary>
        /// <param name="userId">UUID пользователя</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>Список платежей</returns>
        public async Task<List<Payment>> GetByUserAsync(Guid userId, CancellationToken ct)
        {
            return await _dbSet
                .Include(p => p.Ticket)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PaymentCreationDate)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Все платежи по билету (попытки, возвраты),
        /// отсортированные по дате создания.
        /// </summary>
        /// <param name="ticketId">UUID билета</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>Список платежей</returns>
        public async Task<List<Payment>> GetByTicketAsync(Guid ticketId, CancellationToken ct)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.TicketId == ticketId)
                .OrderBy(p => p.PaymentCreationDate)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Платежи по статусу.
        /// </summary>
        /// <param name="status">Статус платежа</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>Список платежей</returns>
        public async Task<List<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken ct)
        {
            return await _dbSet
                .Include(p => p.Ticket)
                .Include(p => p.User)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaymentCreationDate)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Есть ли у билета успешный (подтверждённый) платёж.
        /// </summary>
        /// <param name="ticketId">UUID билета</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>true, если есть подтверждённый платёж</returns>
        public async Task<bool> HasConfirmedPaymentAsync(Guid ticketId, CancellationToken ct)
        {
            return await _dbSet
                .AnyAsync(p => p.TicketId == ticketId
                            && p.Status == PaymentStatus.Confirmed, ct);
        }

        /// <summary>
        /// Сумма всех подтверждённых платежей по билету минус возвраты.
        /// Если возвратов не было — просто сумма подтверждённых.
        /// </summary>
        /// <param name="ticketId">UUID билета</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>Итоговая оплаченная сумма</returns>
        public async Task<decimal> GetTotalPaidAsync(Guid ticketId, CancellationToken ct)
        {
            return await _dbSet
                .Where(p => p.TicketId == ticketId
                         && p.Status == PaymentStatus.Confirmed)
                .SumAsync(p => p.Price, ct);
        }
    }
}