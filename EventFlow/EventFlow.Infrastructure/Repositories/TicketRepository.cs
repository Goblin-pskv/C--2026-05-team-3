using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using EventFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EventFlow.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий билетов на EF Core.
    /// Наследует базовый CRUD из BaseRepository&lt;Ticket&gt;.
    /// </summary>
    public class TicketRepository(EventFlowDbContext context)
        : BaseRepository<Tickets>(context), ITicketRepository
    {
        
        /// <summary>
        /// Получить билет по Id с подгрузкой Event, User и Payments.
        /// </summary>
        /// <param name="id">UUID билета</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>Ticket или null</returns>
        public override async Task<Tickets> GetByIdWithIncludesAsync(Guid id, CancellationToken ct)
        {
            return await _dbSet
                .Include(t => t.Event)
                .Include(t => t.User)
                .Include(t => t.Payments)
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        /// <summary>
        /// Все билеты пользователя (с мероприятием и платежами),
        /// отсортированные по дате создания (новые сверху).
        /// </summary>
        public async Task<List<Tickets>> GetByUserAsync(Guid userId, CancellationToken ct)
        {
            return await _dbSet
                .Include(t => t.Event)
                .Include(t => t.Payments)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TicketCreationDate)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Все билеты на мероприятие (с пользователем и платежами),
        /// отсортированные по номеру билета.
        /// </summary>
        public async Task<List<Tickets>> GetByEventAsync(Guid eventId, CancellationToken ct)
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.Payments)
                .Where(t => t.EventId == eventId)
                .OrderBy(t => t.TicketNumber)
                .ToListAsync(ct);
        }

        ///// <summary>
        ///// Найти билет по QR-коду (для check-in).
        ///// </summary>
        //public async Task<Tickets> GetByQrCodeAsync(string code, CancellationToken ct)
        //{
        //    if (string.IsNullOrWhiteSpace(code))
        //        return null;

        //    return await _dbSet
        //        .Include(t => t.Event)
        //        .Include(t => t.User)
        //        .FirstOrDefaultAsync(t => t.QrCode == code, ct);
        //}

        /// <summary>
        /// Найти билет по уникальному человекочитаемому номеру.
        /// </summary>
        public async Task<Tickets> GetByNumberAsync(string ticketNumber, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(ticketNumber))
                return null;

            return await _dbSet
                .Include(t => t.Event)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, ct);
        }

        /// <summary>
        /// Проверяет, есть ли у пользователя билет на мероприятие.
        /// </summary>
        public async Task<bool> ExistsAsync(Guid userId, Guid eventId, CancellationToken ct)
        {
            return await _dbSet
                .AnyAsync(t => t.UserId == userId && t.EventId == eventId, ct);
        }

        /// <summary>
        /// Билеты по статусу на мероприятие.
        /// </summary>
        public async Task<List<Tickets>> GetByStatusAsync(
            Guid eventId, TicketStatus status, CancellationToken ct)
        {
            return await _dbSet
                .Include(t => t.User)
                .Where(t => t.EventId == eventId && t.status == status)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Количество подтверждённых билетов на мероприятие.
        /// </summary>
        public async Task<int> CountConfirmedAsync(Guid eventId, CancellationToken ct)
        {
            return await _dbSet
                .CountAsync(t => t.EventId == eventId
                              && t.status == TicketStatus.Confirmed, ct);
        }
    }
}