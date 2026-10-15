using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFlow.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы с профилями организаторов в базе данных.
    /// </summary>
    public class OrganizerRepository : BaseRepository<Organizer>, IOrganizerRepository
    {
        /// <summary>
        /// Конструктор репозитория профилей организаторов.
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public OrganizerRepository(EventFlowDbContext context) : base(context)
        {
        }

        public async Task<Organizer?> GetByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _dbSet
                .Where(o => o.UserId == userId && o.IsVerified == false)
                .Select(o => new Organizer
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    CompanyName = o.CompanyName,
                    Description = o.Description,
                    IsVerified = o.IsVerified,
                    UpdatedAt = o.UpdatedAt,
                    CreatedAt = o.CreatedAt,
                    EventsCount = o.Events.Count(),
                    User = o.User,
                    WebSite = o.WebSite,
                    IsDelete = o.IsDelete,
                    Reason = null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Organizer>> GetPendingAsync(int offset, int count, CancellationToken ct)
        {
            return await _dbSet
                .Where(o => o.IsVerified == false && o.IsDelete == false)
                .OrderBy(o => o.CreatedAt)
                .Skip(offset)
                .Take(count)
                .ToListAsync(ct);
        }
    }
}