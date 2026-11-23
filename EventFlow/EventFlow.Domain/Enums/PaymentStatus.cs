using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EventFlow.Domain.Enums
{
    public enum PaymentStatus
    {
        /// <summary>
        /// Ожидает подтверждения. Создано мероприятие, но не куплен билет.
        /// </summary>
        [Description("Ожидание")]
        Pending = 0,

        /// <summary>
        /// Подтвержден.
        /// Оплата прошла (появилась запись в таблице)
        /// </summary>
        [Description("Подтверждена")]
        Confirmed = 1,

        /// <summary>
        /// Отменена. Пользователь отменил или из-за ошибки.
        /// </summary>
        [Description("Отменена")]
        Cancelled = 2,

        /// <summary>
        /// Вернули шекели
        /// </summary>
        [Description("Возврат")]
        Returned = 3,

        /// <summary>
        /// Обработка. Платёж инициирован и ожидает подтверждения банком.
        /// Промежуточный статус между Pending и Confirmed.
        /// </summary>
        [Description("Обработка")]
        Processing = 4
    }
}
