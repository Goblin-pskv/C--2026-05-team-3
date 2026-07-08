using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.DTOs
{
    /// <summary>
    /// DTO для создания нового мероприятия.
    /// Используется в POST /api/events.
    /// 
    /// Содержит только поля, которые может заполнить организатор.
    /// Поля Id, OrganizerId, Status, CreatedAt устанавливаются автоматически.
    /// 
    /// Валидация выполняется через FluentValidation (CreateEventValidator).
    /// </summary>
    public class CreateEventDto
    {
        /// <summary>Название мероприятия (обязательно, max 255)</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Подробное описание (опционально, max 5000)</summary>
        public string? Description { get; set; }

        /// <summary>
        /// Тип мероприятия как строка.
        /// Допустимые значения: Conference, Seminar, Workshop, Sports, Concert, Exhibition, Meetup, Other
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>Дата и время начала (должна быть в будущем)</summary>
        public DateTime Start { get; set; }

        /// <summary>Дата и время окончания (должна быть позже Start)</summary>
        public DateTime End { get; set; }

        /// <summary>Город проведения (обязательно)</summary>
        public string City { get; set; } = string.Empty;

        /// <summary>Точный адрес (обязательно)</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Стоимость участия (>= 0, по умолчанию 0)</summary>
        public decimal Price { get; set; }

        /// <summary>Максимальное количество участников (опционально, > 0)</summary>
        public int? MaxParticipants { get; set; }

        /// <summary>URL изображения обложки (опционально)</summary>
        public string? ImageUrl { get; set; }
    }
}
