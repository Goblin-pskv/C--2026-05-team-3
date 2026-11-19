using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EventFlow.Domain.Enums
{
    public enum EventStatus
    {
        /// <summary>
        /// Черновик. Мероприятие создано, но не опубликовано.
        /// Видно только организатору. Можно редактировать и удалять.
        /// </summary>
        [Description("Черновик")]
        Draft = 0,

        /// <summary>
        /// Опубликовано. Мероприятие видно в каталоге.
        /// Пользователи могут регистрироваться.
        /// Редактирование ограничено (нельзя менять дату/место).
        /// </summary>
        [Description("Опубликовано")]
        Published = 1,

        /// <summary>
        /// Идет сейчас. Дата начала наступила.
        /// Регистрация закрыта. Можно отмечать присутствующих.
        /// </summary>
        [Description("В процессе")]
        InProgress = 2,

        /// <summary>
        /// Завершено. Дата окончания прошла.
        /// Можно собирать отзывы и статистику.
        [Description("Завершено")]
        Completed = 3,

        /// <summary>
        /// Отменено. Организатор отменил мероприятие.
        /// Нужно уведомить участников и вернуть деньги.
        /// </summary>
        [Description("Отменено")]
        Cancelled = 4
    }
}
