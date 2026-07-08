using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.DTOs
{
    /// <summary>
    /// DTO (Data Transfer Object) для отображения информации о мероприятии.
    /// 
    /// Зачем нужен DTO, а не Entity:
    /// 1. Контроль данных — отдаем только то, что нужно клиенту
    /// 2. Безопасность — не отдаем внутренние поля (PasswordHash и т.д.)
    /// 3. Производительность — меньше данных передается по сети
    /// 4. Гибкость — можно изменить структуру Entity без изменения API
    /// 
    /// Отличия от Entity Event:
    /// - OrganizerName вместо OrganizerId (читаемое имя)
    /// - CurrentParticipants вместо подсчета через Registrations
    /// - Status как string вместо enum (удобнее для клиента)
    /// </summary>
    public class EventDto
    {
        /// <summary>Уникальный идентификатор мероприятия (UUID)</summary>
        public Guid Id { get; set; }

        /// <summary>Название мероприятия</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Подробное описание</summary>
        public string? Description { get; set; }

        /// <summary>Тип мероприятия (Conference, Seminar, Sports и т.д.)</summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>Дата и время начала</summary>
        public DateTime Start { get; set; }

        /// <summary>Дата и время окончания</summary>
        public DateTime End { get; set; }

        /// <summary>Город проведения</summary>
        public string City { get; set; } = string.Empty;

        /// <summary>Точный адрес</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Стоимость участия (0 = бесплатно)</summary>
        public decimal Price { get; set; }

        /// <summary>Максимальное количество участников (null = без лимита)</summary>
        public int? MaxParticipants { get; set; }

        /// <summary>Текущее количество подтвержденных участников</summary>
        public int CurrentParticipants { get; set; }

        /// <summary>Количество свободных мест</summary>
        public int AvailableSpots { get; set; }

        /// <summary>Текущий статус мероприятия</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>URL изображения обложки</summary>
        public string? ImageUrl { get; set; }

        /// <summary>Название организации-организатора</summary>
        public string OrganizerName { get; set; } = string.Empty;

        /// <summary>Опубликовано ли мероприятие</summary>
        public bool IsPublished { get; set; }

        /// <summary>Дата создания</summary>
        public DateTime CreatedAt { get; set; }
    }
}
