using EventFlow.Application.Interfaces;
using EventFlow.Domain.Common;
using EventFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Infrastructure.Repositories
{

    /// <summary>
    /// Базовый репозиторий с общей логикой для всех сущностей.
    /// 
    /// Зачем нужен:
    /// 1. Избегаем дублирования кода (CRUD операции одинаковы для всех сущностей)
    /// 2. Централизованная работа с DbContext
    /// 3. Специализированные репозитории наследуются от него
    /// 
    /// Как использовать:
    /// - EventRepository : BaseRepository&lt;Event&gt;, IEventRepository
    /// - UserRepository : BaseRepository&lt;User&gt;, IUserRepository
    /// 
    /// Generic-параметр T:
    /// - Должен наследоваться от BaseEntity (чтобы иметь Id, CreatedAt, UpdatedAt)
    /// - Должен быть ссылочным типом (class)
    /// </summary>
    /// <typeparam name="T">Тип сущности (Event, User, Organizer, Registration)</typeparam>
    public class BaseRepository<T> : IRepository<T>, IDisposable where T : class
    {
        /// <summary>
        /// Контекст базы данных.
        /// Protected, чтобы наследники могли использовать его для сложных запросов.
        /// </summary>
        protected readonly EventFlowDbContext _context;

        /// <summary>
        /// DbSet для работы с сущностями типа T.
        /// Это "виртуальная таблица" в БД для сущностей типа T.
        /// </summary>
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Конструктор базового репозитория.
        /// </summary>
        /// <param name="context">Контекст БД (внедряется через DI)</param>
        public BaseRepository(EventFlowDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Получить сущность по ID.
        /// Базовая реализация — простой FindAsync.
        /// Наследники могут переопределить для Include связанных данных.
        /// </summary>
        /// <param name="id">UUID сущности</param>
        /// <returns>Сущность или null, если не найдена</returns>
        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _dbSet.FindAsync(new object[] { id }, ct);

        }

        /// <summary>
        /// Получить все сущности.
        /// Осторожно: может вернуть много данных. Используйте с фильтрацией.
        /// </summary>
        /// <returns>Список всех сущностей</returns>
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct)
        {
            return await _dbSet.ToListAsync(ct);
        }

        /// <summary>
        /// Добавить новую сущность в БД.
        /// После вызова нужно вызвать SaveChangesAsync().
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// Метод возвращает Task (он асинхронный для вызывающего кода), 
        /// но внутри себя он не тратит ресурсы на лишний await
        public async virtual Task AddAsync(T entity, CancellationToken ct)
        {
            await _dbSet.AddAsync(entity,ct);
        }

        /// <summary>
        /// Добавить несколько сущностей.
        /// Более эффективно, чем добавление по одной.
        /// </summary>
        /// <param name="entities">Коллекция сущностей</param>
        /// Метод возвращает Task (он асинхронный для вызывающего кода), 
        /// но внутри себя он не тратит ресурсы на лишний await
        public async virtual Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct)
        {
            await _dbSet.AddRangeAsync(entities, ct);
        }

        /// <summary>
        /// Обновить сущность.
        /// EF Core отслеживает изменения автоматически, но для отсоединенных
        /// сущностей нужно явно вызвать Update.
        /// </summary>
        /// <param name="entity">Сущность с измененными данными</param>
        public void UpdateAsync(T entity, CancellationToken ct)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Удалить сущность.
        /// </summary>
        /// <param name="entity">Сущность для удаления</param>
        public virtual void Delete(T entity, CancellationToken ct)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Удалить сущность по ID.
        /// Удобно, когда не нужно загружать сущность перед удалением.
        /// </summary>
        /// <param name="id">UUID сущности</param>
        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var entity = await GetByIdAsync(id, ct);
            if (entity != null)
            {
                Delete(entity, ct);
            }
        }

        /// <summary>
        /// Сохранить все изменения в БД.
        /// Выполняет COMMIT транзакции.
        /// </summary>
        /// <returns>Количество измененных записей</returns>
        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _context.SaveChangesAsync(ct);
        }


        /// <summary>
        /// Освобождение ресурсов (паттерн IDisposable).
        /// DbContext освобождается автоматически через DI,
        /// но этот метод нужен для явного управления.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
