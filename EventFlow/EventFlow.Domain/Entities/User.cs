using EventFlow.Domain.Common;
using EventFlow.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Entities
{
    /// <summary>
    /// Пользователь системы.
    /// Может быть участником (Participant) или организатором (Organizer).
    /// </summary>
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;  
        public Organizer? OrganizerProfile { get; set; }
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public string FullName => $"{FirstName} {LastName}";

    }
}
