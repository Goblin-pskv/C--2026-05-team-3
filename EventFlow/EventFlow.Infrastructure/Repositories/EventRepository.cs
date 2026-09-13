using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using EventFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFlow.Infrastructure.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы с мероприятиями.
    /// Наследуется от BaseRepository&lt;Event&gt; и реализует IEventRepository.
    /// </summary>
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(EventFlowDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Получить мероприятие по ID с загрузкой связанных данных
        /// </summary>
        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(e => e.Organizer)
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Получить список всех опубликованных мероприятий
        /// </summary>
        public async Task<List<Event>> GetPublishedEventsAsync()
        {
            return await _dbSet
                .Where(e => e.IsPublished && e.Status == EventStatus.Published)
                .OrderBy(e => e.Start)
                .ToListAsync();
        }

        /// <summary>
        /// Получить все мероприятия конкретного организатора
        /// </summary>
        public async Task<List<Event>> GetByOrganizerIdAsync(Guid organizerId)
        {
            return await _dbSet
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Добавить новое мероприятие
        /// </summary>
        public async Task AddAsync(Event @event)
        {
            await _dbSet.AddAsync(@event);
        }

        /// <summary>
        /// Обновить мероприятие
        /// </summary>
        public void Update(Event @event)
        {
            _dbSet.Update(@event);
        }

        /// <summary>
        /// Удалить мероприятие
        /// </summary>
        public void Delete(Event @event)
        {
            _dbSet.Remove(@event);
        }

        /// <summary>
        /// Сохранить изменения в БД
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}