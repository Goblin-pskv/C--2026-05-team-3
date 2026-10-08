using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Enums
{
    public enum TicketStatus
    {
        /// <summary>
        /// Ожидает подтверждения. Создана, но не подтверждена.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Подтвержден.
        /// Оплата прошла (появилась запись в таблице)
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Отменена. Пользователь или организатор отменил билет.
        /// ???? я хз мб после возвращения денег ????
        /// </summary>
        Cancelled = 2,

        /// <summary>
        /// Просрочен
        /// Если билет не был подтвержден до конца мероприятия
        /// </summary>
        Expired = 3
    }
}
