using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Enums
{
    /// <summary>
    /// Роли пользователей в системе.
    /// Определяет права доступа:
    /// - Participant: может регистрироваться на мероприятия
    /// - Organizer: может создавать и управлять мероприятиями
    /// 
    public enum UserRole
    {
        Participant = 0,
        Organizer = 1
    }
}
