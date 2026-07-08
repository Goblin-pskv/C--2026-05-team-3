using EventFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{   
    /// <summary>
    /// Контракт для работы с мероприятиями в базе данных.
    /// 
    /// Этот интерфейс определяет операции, которые должен уметь выполнять
    /// репозиторий мероприятий. Реализация находится в Infrastructure слое
    /// (EventRepository) и использует Entity Framework Core для работы с БД.
    /// 
    /// Зачем нужен интерфейс:
    /// 1. Разделение абстракции и реализации (принцип Dependency Inversion)
    /// 2. Возможность подменить реализацию для unit-тестов (mock)
    /// 3. Application слой не зависит от конкретной ORM (EF Core)
    /// 4. Легко заменить PostgreSQL на другую БД без изменения бизнес-логики
    /// 
    /// Как использовать:
    /// - Внедрите через DI: public EventService(IEventRepository repository)
    /// - Все методы асинхронные для производительности
    /// - Методы возвращают Domain-сущности, а не DTO
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Получить мероприятие по уникальному идентификатору.
        /// Возвращает мероприятие с загруженными связанными данными
        /// (организатор, регистрации).
        /// </summary>
        /// <param name="id">UUID мероприятия</param>
        /// <returns>Мероприятие или null, если не найдено</returns>
        /// <example>
        /// var @event = await _repository.GetByIdAsync(eventId);
        /// if (@event == null) throw new DomainException("Не найдено");
        /// </example>
        Task<Event?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить список всех опубликованных мероприятий.
        /// Возвращает только мероприятия со статусом Published.
        /// Используется для отображения каталога мероприятий.
        /// </summary>
        /// <returns>Список опубликованных мероприятий</returns>
        Task<List<Event>> GetPublishedEventsAsync();

        /// <summary>
        /// Получить все мероприятия конкретного организатора.
        /// Включает черновики, опубликованные и отмененные.
        /// Используется в личном кабинете организатора.
        /// </summary>
        /// <param name="organizerId">UUID организатора</param>
        /// <returns>Список мероприятий организатора</returns>
        Task<List<Event>> GetByOrganizerIdAsync(Guid organizerId);

        /// <summary>
        /// Добавить новое мероприятие в базу данных.
        /// После вызова нужно вызвать SaveChangesAsync() для сохранения.
        /// </summary>
        /// <param name="event">Сущность мероприятия для добавления</param>
        Task AddAsync(Event @event);

        /// <summary>
        /// Обновить существующее мероприятие.
        /// EF Core автоматически отслеживает изменения.
        /// После вызова нужно вызвать SaveChangesAsync().
        /// </summary>
        /// <param name="event">Мероприятие с измененными данными</param>
        void Update(Event @event);

        /// <summary>
        /// Удалить мероприятие из базы данных.
        /// Используется для удаления черновиков.
        /// После вызова нужно вызвать SaveChangesAsync().
        /// </summary>
        /// <param name="event">Мероприятие для удаления</param>
        void Delete(Event @event);

        /// <summary>
        /// Сохранить все изменения в базе данных.
        /// Выполняет COMMIT транзакции.
        /// Вызывается после Add/Update/Delete операций.
        /// </summary>
        Task SaveChangesAsync();
        
    }
}
