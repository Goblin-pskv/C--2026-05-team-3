using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.DTOs
{
    /// <summary>
    /// DTO для отображения информации о пользователе.
    /// 
    /// ВАЖНО: НЕ содержит PasswordHash и другие чувствительные данные!
    /// Используется для отображения в профиле, списке участников и т.д.
    /// </summary>
    public class UserDto
    {
        /// <summary>Уникальный идентификатор (UUID)</summary>
        public Guid Id { get; set; }

        /// <summary>Имя пользователя</summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>Фамилия пользователя</summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>Email</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Номер телефона (опционально)</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Роль пользователя (Participant или Organizer)</summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>Вычисляемое полное имя</summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>Дата регистрации</summary>
        public DateTime CreatedAt { get; set; }
    }
}
