using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.DTOs
{
    /// <summary>
    /// DTO для обновления существующего мероприятия.
    /// Используется в PUT /api/events/{id}.
    /// 
    /// Все поля опциональны — обновляются только те, что переданы.
    /// Нельзя изменить статус через этот DTO (для этого есть отдельные методы).
    /// </summary>
    public class UpdateEventDto
    {
        /// <summary>Новое название (опционально)</summary>
        public string? Title { get; set; }

        /// <summary>Новое описание (опционально)</summary>
        public string? Description { get; set; }

        /// <summary>Новая дата начала (опционально)</summary>
        public DateTime? Start { get; set; }

        /// <summary>Новая дата окончания (опционально)</summary>
        public DateTime? End { get; set; }

        /// <summary>Новый город (опционально)</summary>
        public string? City { get; set; }

        /// <summary>Новый адрес (опционально)</summary>
        public string? Address { get; set; }

        /// <summary>Новая цена (опционально)</summary>
        public decimal? Price { get; set; }

        /// <summary>Новый лимит участников (опционально)</summary>
        public int? MaxParticipants { get; set; }

        /// <summary>Новый URL изображения (опционально)</summary>
        public string? ImageUrl { get; set; }
    }
}
