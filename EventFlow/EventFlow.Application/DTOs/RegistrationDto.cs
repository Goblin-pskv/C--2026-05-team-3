using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.DTOs
{
    /// <summary>
    /// DTO для отображения информации о регистрации на мероприятие.
    /// Содержит данные как о регистрации, так и о связанном мероприятии/пользователе.
    /// </summary>
    public class RegistrationDto
    {
        /// <summary>Уникальный идентификатор регистрации (UUID)</summary>
        public Guid Id { get; set; }

        /// <summary>ID мероприятия</summary>
        public Guid EventId { get; set; }

        /// <summary>Название мероприятия (для удобства отображения)</summary>
        public string EventTitle { get; set; } = string.Empty;

        /// <summary>Дата начала мероприятия</summary>
        public DateTime EventStart { get; set; }

        /// <summary>ID пользователя</summary>
        public Guid UserId { get; set; }

        /// <summary>Полное имя пользователя</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Email пользователя</summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>Статус регистрации (Pending, Confirmed, Cancelled, Attended)</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Дата создания регистрации</summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>Дата подтверждения (null если не подтверждена)</summary>
        public DateTime? ConfirmationDate { get; set; }
    }
}
