using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EventFlow.Domain.Enums
{
    public enum TicketStatus
    {
        [Description("Ожидание")]
        Pending = 0,

        [Description("Подтверждён")]
        Confirmed = 1,

        [Description("Отменён")]
        Cancelled = 2,

        [Description("Просрочен")]
        Expired = 3
    }
}
