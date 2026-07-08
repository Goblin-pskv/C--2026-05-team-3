using System;
using System.Collections.Generic;
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
        Conference = 0, // конференция
        Seminar = 1,  // семинар
        Workshop = 2, // мастер-класс
        Sports = 3, // спортивное мероприятие
        Concert = 4, // концерт, музыкальное
        Exhibition = 5, // выставка
        Meetup = 6, // встреча, неформальное мероприятие
        Birthday = 7, // день рождения
        Funeral = 8, // похороны
        Wedding = 9, // свадьба
        Other = 99 // другое
    }
}
