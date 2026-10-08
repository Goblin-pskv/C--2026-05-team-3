using EventFlow.Domain.Common;
using EventFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Entities
{
    public class Payment : BaseEntity
    {

        /// <summary>
        /// ID платежа.
        /// </summary>
        public Guid PaymentId { get; set; }
        /// <summary>
        /// ID билета, к которому относится платёж.
        /// Внешний ключ на таблицу Tickets.
        /// </summary>
        public Guid TicketId { get; set; }

        /// <summary>
        /// ID пользователя-плательщика.
        /// Внешний ключ на таблицу Users.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Дата и время создания платежа (UTC).
        /// </summary>
        public DateTime PaymentCreationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата последнего изменения (например, подтверждение, отмена).
        /// </summary>
        public DateTime? LastModificationDate { get; set; }
        /// <summary>
        /// Сумма платежа.
        /// Положительная — оплата, отрицательная — возврат.
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Статус платежа.
        /// </summary>
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        /// <summary>
        /// Дата фактического проведения платежа (UTC).
        /// Заполняется при переходе в статус Success.
        /// </summary>
        public DateTime? PaidAt { get; set; }

        /// <summary>
        /// Билет, к которому относится платёж.
        /// </summary>
        public virtual Tickets Ticket { get; set; } = null!;

        /// <summary>
        /// Пользователь-плательщик.
        /// </summary>
        public virtual User User { get; set; } = null!;
    }
}
