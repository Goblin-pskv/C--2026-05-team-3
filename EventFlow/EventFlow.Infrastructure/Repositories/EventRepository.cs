using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Infrastructure.Repositories
{
    public class EventRepository(EventFlowDbContext context) : BaseRepository<Event>(context), IEventRepository
    {
        /// <summary>
        /// Реализация метода получения сущности со связанными данными для Event.
        /// </summary>
        /// <param name="id">UUID мероприятия</param>>
        /// <param name="ct"></param>
        /// <returns>Event или null, если не найдена</returns>
        public override async Task<Event?> GetByIdWithIncludesAsync(Guid id, CancellationToken ct)
        {
            return await _dbSet
                .Include(e => e.Organizer)
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        /// <summary>
        /// Возвращает информацию о мероприятии, включая список регистраций, по id.
        /// </summary>
        /// <param name="eventId">UUID мероприятия</param>>
        /// <param name="ct"></param>
        /// <returns>Event или null, если не найдена</returns>
        public async Task<Event?> GetEventWithRegistrationsAsync(Guid eventId, CancellationToken ct)
        {
            return await _dbSet
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == eventId, ct);
        }

        /// <summary>
        /// Возвращает детализированный списов мероприятий с пагинацией.
        /// </summary>
        /// <param name="page">Порядковый номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<List<Event>> GetEventsWithDetailsAsync(
            int page, int pageSize, DateTime dateStart, DateTime dateEnd, CancellationToken ct)
        {
            return await _dbSet
                .Include(e => e.Registrations)
                .Include(e => e.Organizer)
                .Where(e => e.Start >= dateStart || e.End <= dateEnd)
                .OrderBy(e => e.Start)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<List<Event>> GetPublishedEventsAsync(CancellationToken ct)
        {
            return await _dbSet
                .Where(e => e.IsPublished)
                .ToListAsync(ct);
        }

        public async Task<List<Event>> GetByOrganizerIdAsync(Guid organizerId, CancellationToken ct)
        {
            return await _dbSet
                .Where(e => e.OrganizerId == organizerId)
                .ToListAsync(ct);
        }
    }
}
