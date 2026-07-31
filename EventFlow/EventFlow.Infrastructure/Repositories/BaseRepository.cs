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
    public class BaseRepository<T> : IRepository<T> where T: BaseEntity, IDisposable
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
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);

        }

        /// <summary>
        /// Получить все сущности.
        /// Осторожно: может вернуть много данных. Используйте с фильтрацией.
        /// </summary>
        /// <returns>Список всех сущностей</returns>
        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(_dbSet.AsEnumerable());
        }

        /// <summary>
        /// Добавить новую сущность в БД.
        /// После вызова нужно вызвать SaveChangesAsync().
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// Метод возвращает Task (он асинхронный для вызывающего кода), 
        /// но внутри себя он не тратит ресурсы на лишний await
        public virtual Task AddAsync(T entity)
        {
            _dbSet.Add(entity);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Добавить несколько сущностей.
        /// Более эффективно, чем добавление по одной.
        /// </summary>
        /// <param name="entities">Коллекция сущностей</param>
        /// Метод возвращает Task (он асинхронный для вызывающего кода), 
        /// но внутри себя он не тратит ресурсы на лишний await
        public virtual Task AddRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Обновить сущность.
        /// EF Core отслеживает изменения автоматически, но для отсоединенных
        /// сущностей нужно явно вызвать Update.
        /// </summary>
        /// <param name="entity">Сущность с измененными данными</param>
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Удалить сущность.
        /// </summary>
        /// <param name="entity">Сущность для удаления</param>
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Удалить сущность по ID.
        /// Удобно, когда не нужно загружать сущность перед удалением.
        /// </summary>
        /// <param name="id">UUID сущности</param>
        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// Сохранить все изменения в БД.
        /// Выполняет COMMIT транзакции.
        /// </summary>
        /// <returns>Количество измененных записей</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
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
