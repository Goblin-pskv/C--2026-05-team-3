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
    public interface IEventRepository : IRepository<Event>
    {
        Task<List<Event>> GetPublishedEventsAsync(CancellationToken ct);
        Task<List<Event>> GetByOrganizerIdAsync(Guid organizerId, CancellationToken ct);
        Task<Event?> GetEventWithRegistrationsAsync(Guid eventId, CancellationToken ct);
        Task<List<Event>> GetEventsWithDetailsAsync(
            int page, int pageSize, DateTime dateStart, DateTime dateEnd, CancellationToken ct);

    }
}
