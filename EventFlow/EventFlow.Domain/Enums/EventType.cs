using System;
using System.ComponentModel;
using System.Text;

namespace EventFlow.Domain.Enums
{
    /// <summary>
    /// Типы мероприятий, которые можно создать
    /// Используется для:
    /// - Фильтрации мероприятий в каталоге
    /// - Статистики по типам
    /// - Разной логики для разных типов
    /// </summary>
    public enum EventType
    {
        [Description("Конференция")]
        Conference = 0,

        [Description("Семинар")]
        Seminar = 1,

        [Description("Мастер-класс")]
        Workshop = 2,

        [Description("Спортивное мероприятие")]
        Sports = 3,

        [Description("Концерт")]
        Concert = 4,

        [Description("Выставка")]
        Exhibition = 5,

        [Description("Встреча")]
        Meetup = 6,

        [Description("День рождения")]
        Birthday = 7,

        [Description("Похороны")]
        Funeral = 8,

        [Description("Свадьба")]
        Wedding = 9,

        [Description("Другое")]
        Other = 99
    }
}
