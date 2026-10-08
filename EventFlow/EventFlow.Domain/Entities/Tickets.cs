using EventFlow.Domain.Common;
using EventFlow.Domain.Enums;
using EventFlow.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace EventFlow.Domain.Entities
{
    /// <summary>
    /// Сохранение записей о билетах.
    /// Связывает User и Event — кто записался на что.
    /// 
    /// Связи:
    /// - Одна User принадлежит многим Tickets (N:1)
    /// - Одна Event принадлежит многим Tickets (N:1)
    /// - Уникальное ограничение: один User = одна Registration на Event
    /// </summary>
    public class Tickets : BaseEntity
    {
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        /// <summary>
        /// ID билета.
        /// </summary>
        public Guid TicketId { get; set; }

        /// <summary>
        /// ID мероприятия, на которое регистрируется пользователь.
        /// Внешний ключ на таблицу Events.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// ID пользователя, который регистрируется.
        /// Внешний ключ на таблицу Users.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Дата и время создания регистрации (UTC).
        /// </summary>
        public DateTime TicketCreationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата последнего изменения.
        /// Возможно для просрочки, отмены билета
        /// </summary>
        public DateTime? LastModificationDate { get; set; }

        /// <summary>
        /// Человекочитаемый уникальный номер билета.
        /// Например: "EVT-2026-000123". Удобно для поиска и печати.
        /// </summary>
        public string TicketNumber { get; set; } = null!;

        /// <summary>
        /// Статус билета.
        /// </summary>
        public TicketStatus status { get; set; } = TicketStatus.Pending;

        /// <summary>
        /// Цена билета на момент покупки (фиксируется, чтобы не зависеть от будущих изменений Event).
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Номер места.
        /// </summary>
        public string? SeatNumber { get; set; }

        /// <summary>
        /// Ряд.
        /// </summary>
        public string? RowNumber { get; set; }

        /// <summary>
        /// Сектор / зона.
        /// </summary>
        public string? Sector { get; set; }

        public virtual Event Event { get; set; } = null!;

        public virtual User User { get; set; } = null!;
        
    }
}
