using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using EventFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Infrastructure.Repositories
{
    public class RegistrationRepository(EventFlowDbContext context) : BaseRepository<Registration>(context), IRegistrationRepository
    {
        /// <summary>
        /// Возвращает регистрацию по id пользователя и id события.
        /// </summary>
        /// <param name="userId">UUID пользователя</param>
        /// <param name="eventId">UUID мероприятия</param>
        /// <returns>Регистрация на событие</returns>
        public async Task<Registration?> GetRegistrationAsync(Guid userId, Guid eventId, CancellationToken ct)
        {
            return await _dbSet.FirstOrDefaultAsync(_ => _.UserId == userId && _.EventId == eventId, ct);
        }

        /// Возвращает список регистраций пользователя по id.
        /// </summary>
        /// <param name="userId">UUID пользователя</param>>
        /// <param name="ct"></param>
        /// <returns>Список регистраций</returns>
        public async Task<List<Registration>> GetRegistrationsByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _dbSet.Where(_ => _.UserId == userId)
                               .ToListAsync();
        }

        /// <summary>
        /// Возвращает список участников события по id.
        /// </summary>
        /// <param name="eventId">UUID мероприятия</param>>
        /// <param name="ct"></param>
        /// <returns>Список регистраций</returns>
        public async Task<List<Registration>> GetRegistrationsByEventIdAsync(Guid eventId, CancellationToken ct)
        {
            return await _dbSet.Where(_ => _.EventId == eventId)
                               .ToListAsync();
        }

        public async Task<int> CountConfirmedAsync(Guid eventId, CancellationToken ct)
        {
            return await _dbSet
                .CountAsync(r => r.EventId == eventId
                              && (r.Status == RegistrationStatus.Confirmed
                               || r.Status == RegistrationStatus.Attended), ct);
        }

    }
}
