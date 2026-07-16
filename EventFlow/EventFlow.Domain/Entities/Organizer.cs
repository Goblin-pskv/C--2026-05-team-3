using EventFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Entities
{
    /// <summary>
    /// Профиль организатора мероприятий.
    /// Расширяет информацию о пользователе, который создает мероприятия.
    /// 
    /// </summary>
    /// Связи:
    /// - Один Organizer принадлежит одному User (1:1)
    /// - Один Organizer может создать много Events (1:N)
    public class Organizer : BaseEntity
    {
        /// <summary>
        /// ID пользователя, которому принадлежит этот профиль.
        /// Связывает Organizer с User (внешний ключ).\
        /// </summary>
        public Guid UserId { get; set; }

        public string? CompanyName { get; set; }

        /// <summary>
        /// Описание организатора.
        /// </summary>
        public string? Description { get; set; }
        public string? WebSite {  get; set; }

        /// <summary>
        /// Флаг верификации организатора.
        /// true = документы проверены администрацией
        /// false = еще не проверен или отклонен
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Пользователь, которому принадлежит этот профиль.
        /// Навигационное свойство для EF Core.
        /// Связь: один Organizer принадлежит одному User (1:1)
        /// </summary>
        public virtual User User { get; set; } = null!;
        /// <summary>
        /// Список всех мероприятий, созданных этим организатором.
        /// Показывает, какие мероприятия организовал этот человек/компания.
        /// Связь: один Organizer может создать много Events (1:N)
        /// </summary>
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
