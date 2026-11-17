using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Infrastructure.Repositories
{
    public class OrganizerRequestRepository : BaseRepository<OrganizerRequest>, IOrganizerRequestRepository
    {
        public OrganizerRequestRepository(EventFlowDbContext context) : base(context)
        {
        }

        public async Task<OrganizerRequest?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _dbSet
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == Domain.Enums.OrganizerRequestStatus.Pending, ct);
        }

        public async Task<IEnumerable<OrganizerRequest>> GetPendingAsync(int offset, int count, CancellationToken ct)
        {
            return await _dbSet
                .Include(r => r.User)
                .Where(o => o.Status == Domain.Enums.OrganizerRequestStatus.Pending)
                .OrderBy(o => o.CreatedAt)
                .Skip(offset)
                .Take(count)
                .ToListAsync(ct);
        }
    }
}
