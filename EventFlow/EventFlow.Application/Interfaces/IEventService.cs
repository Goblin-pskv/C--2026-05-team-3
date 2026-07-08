using EventFlow.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    /// <summary>
    /// Контракт для бизнес-логики работы с мероприятиями.
    /// 
    /// Этот сервис координирует работу между:
    /// - Репозиториями (получение/сохранение данных)
    /// - Domain-сущностями (бизнес-правила)
    /// - DTO (передача данных между слоями)
    /// 
    /// Отличие от репозитория:
    /// - Repository: работа с БД (CRUD операции)
    /// - Service: бизнес-логика (проверки, вычисления, оркестрация)
    /// 
    /// Пример:
    /// При создании мероприятия сервис проверяет:
    /// 1. Существует ли организатор
    /// 2. Валидны ли даты
    /// 3. Заполнены ли обязательные поля
    /// И только потом вызывает репозиторий для сохранения.
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Создать новое мероприятие.
        /// Выполняет валидацию и сохраняет в БД.
        /// </summary>
        /// <param name="dto">Данные для создания мероприятия</param>
        /// <param name="organizerId">ID организатора из JWT токена</param>
        /// <returns>ID созданного мероприятия</returns>
        /// <exception cref="DomainException">Если валидация не пройдена</exception>
        Task<Guid> CreateEventAsync(CreateEventDto dto, Guid organizerId);

        /// <summary>
        /// Получить мероприятие по ID с полной информацией.
        /// </summary>
        /// <param name="id">UUID мероприятия</param>
        /// <returns>DTO мероприятия или null</returns>
        Task<EventDto?> GetEventByIdAsync(Guid id);

        /// <summary>
        /// Получить список всех опубликованных мероприятий.
        /// Используется для каталога.
        /// </summary>
        /// <returns>Список DTO мероприятий</returns>
        Task<List<EventDto>> GetPublishedEventsAsync();

        /// <summary>
        /// Получить мероприятия конкретного организатора.
        /// </summary>
        /// <param name="organizerId">ID организатора</param>
        /// <returns>Список DTO мероприятий</returns>
        Task<List<EventDto>> GetEventsByOrganizerAsync(Guid organizerId);

        /// <summary>
        /// Опубликовать мероприятие (перевести в статус Published).
        /// Делает мероприятие видимым в каталоге.
        /// </summary>
        /// <param name="eventId">ID мероприятия</param>
        /// <param name="organizerId">ID организатора (проверка прав)</param>
        /// <exception cref="DomainException">Если нет прав или мероприятие не готово</exception>
        Task PublishEventAsync(Guid eventId, Guid organizerId);

        /// <summary>
        /// Отменить мероприятие.
        /// Уведомляет всех зарегистрированных участников.
        /// </summary>
        /// <param name="eventId">ID мероприятия</param>
        /// <param name="organizerId">ID организатора (проверка прав)</param>
        Task CancelEventAsync(Guid eventId, Guid organizerId);

        /// <summary>
        /// Обновить данные мероприятия.
        /// </summary>
        /// <param name="eventId">ID мероприятия</param>
        /// <param name="dto">Новые данные</param>
        /// <param name="organizerId">ID организатора (проверка прав)</param>
        Task UpdateEventAsync(Guid eventId, UpdateEventDto dto, Guid organizerId);
    }
}
