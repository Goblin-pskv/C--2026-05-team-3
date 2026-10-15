using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.DTOs
{
    public class OrganizerDto
    {
        /// <summary>Название компании</summary>
        public string? CompanyName { get; set; }

        /// <summary>Описание организатора</summary>
        public string? Description { get; set; }

        /// <summary>Веб-сайт</summary>
        public string? WebSite { get; set; }

        /// <summary>Пройдена ли верификация</summary>
        public bool IsVerified { get; set; }

        /// <summary>Количество созданных мероприятий</summary>
        public int EventsCount { get; set; }
    }
}
