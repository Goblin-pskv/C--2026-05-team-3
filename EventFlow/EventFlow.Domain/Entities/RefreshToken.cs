using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; } // истекает в
        public DateTime CreatedAt { get; set; } // создан в
        public bool IsRevoked { get; set; } // отозван
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt; // истек срок действия
        public bool isActive => !IsRevoked && !IsExpired;

        public User User { get; set; } = null!;


    }
}
