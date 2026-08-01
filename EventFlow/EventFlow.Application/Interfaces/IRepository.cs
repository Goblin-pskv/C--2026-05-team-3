using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    public interface IRepository<T> where T: class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);

        Task AddAsync(T entity, CancellationToken ct);

        void UpdateAsync(T item, CancellationToken ct);
        void Delete(T entity, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
