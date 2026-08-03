using EventFlow.Domain.Common;
using EventFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Infrastructure.Data
{

    /// <summary>
    /// Контекст базы данных для EventFlow.
    /// 
    /// Это главная точка взаимодействия с PostgreSQL через Entity Framework Core.
    /// Через этот класс выполняются все операции чтения/записи в БД.
    /// 
    /// Что делает:
    /// 1. Управляет подключением к базе данных
    /// 2. Отслеживает изменения сущностей (Change Tracker)
    /// 3. Генерирует SQL запросы на основе LINQ
    /// 4. Выполняет миграции (Code First подход)
    /// 5. Автоматически заполняет CreatedAt/UpdatedAt через override SaveChangesAsync
    /// 
    /// Как использовать:
    /// - Внедряется через DI: public EventService(EventFlowDbContext context)
    /// - DbSet<T> — это коллекция сущностей типа T в БД
    /// - После изменений нужно вызвать SaveChangesAsync() для COMMIT
    /// </summary>
    public class EventFlowDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public EventFlowDbContext(DbContextOptions<EventFlowDbContext> options) : base(options)
        {
        }

        /// <summary>Таблица профилей организаторов</summary>
        public DbSet<Organizer> Organizers => Set<Organizer>();

        /// <summary>Таблица мероприятий</summary>
        public DbSet<Event> Events => Set<Event>();

        /// <summary>Таблица регистраций на мероприятия</summary>
        public DbSet<Registration> Registrations => Set<Registration>();

        /// <summary>Таблица refresh-токенов для авторизации</summary>
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        /// <summary>
        /// Настройка модели БД: таблицы, связи, индексы, ограничения.
        /// Конфигурации вынесены в отдельные классы (EventConfiguration и т.д.)
        /// для лучшей читаемости и поддерживаемости.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Применяем все конфигурации из сборки Infrastructure
            // Автоматически находит все классы, реализующие IEntityTypeConfiguration<T>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventFlowDbContext).Assembly);
        }

        /// <summary>
        /// Переопределение SaveChangesAsync для автоматического заполнения
        /// полей CreatedAt и UpdatedAt.
        /// 
        /// Как работает:
        /// 1. Перед сохранением проверяем все измененные сущности
        /// 2. Если сущность добавлена (Added) → устанавливаем CreatedAt и UpdatedAt
        /// 3. Если сущность изменена (Modified) → обновляем только UpdatedAt
        /// 4. Все даты в UTC для корректной работы в разных часовых зонах
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // Новая сущность — устанавливаем обе даты
                        entry.Entity.CreatedAt = now;
                        entry.Entity.UpdatedAt = now;
                        break;

                    case EntityState.Modified:
                        // Измененная сущность — обновляем только UpdatedAt
                        entry.Entity.UpdatedAt = now;
                        // CreatedAt не трогаем — это дата создания
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
