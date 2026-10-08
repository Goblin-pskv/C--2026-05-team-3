using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Enums
{
    public enum PaymentStatus
    {
        /// <summary>
        /// Ожидает подтверждения. Создано мероприятие, но не куплен билет.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Подтвержден.
        /// Оплата прошла (появилась запись в таблице)
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Отменена. Пользователь отменил или из-за ошибки.
        /// </summary>
        Cancelled = 2,

        /// <summary>
        /// Вернули шекели
        /// </summary>
        Returned = 3
    }
}
