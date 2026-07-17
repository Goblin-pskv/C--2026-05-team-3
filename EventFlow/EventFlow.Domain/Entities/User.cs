using EventFlow.Domain.Common;
using EventFlow.Domain.Enums;
using EventFlow.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Entities
{
    /// <summary>
    /// Пользователь системы.
    /// Может быть участником (Participant) или организатором (Organizer).
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Конструктор для бизнес создания модели
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <param name="role"></param>
        /// <param name="phoneNumber"></param>
        public User(string firstName, string lastName, string email, string passwordHash, UserRole role = UserRole.User, string? phoneNumber = null)
            : base(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow)
        {
            FirstName = firstName ?? throw new DomainException($"Null у параметра:{nameof(firstName)}");
            LastName = lastName ?? throw new DomainException($"Null у параметра:{nameof(lastName)}");
            Email = email ?? throw new DomainException($"Null у параметра:{nameof(email)}");
            PhoneNumber = phoneNumber;
            PasswordHash = passwordHash ?? throw new DomainException($"Null у параметра:{nameof(passwordHash)}");
            Role = role;
            OrganizerProfile = null;
            _registrationList = new List<Registration>();
        }
        /// <summary>
        /// Конструктор для EF Core
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="createdAt"></param>
        /// <param name="updatetAt"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <param name="registrations"></param>
        /// <param name="role"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="organizer"></param>
        private User(Guid userId, DateTime createdAt, DateTime updatetAt, 
            string firstName, string lastName, string email,
            string passwordHash, List<Registration> registrations,
            UserRole role, string? phoneNumber, Organizer? organizer) 
            : base(userId, createdAt, updatetAt)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            PasswordHash = passwordHash;
            Role = role;
            OrganizerProfile = organizer;
            _registrationList = registrations ?? new List<Registration>();
        }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email{ get; private set; }
        public string? PhoneNumber{ get; private set; }
        public string PasswordHash{ get; private set; }
        public UserRole Role{ get; private set; }
        public Organizer? OrganizerProfile{ get; private set; }
        private readonly List<Registration> _registrationList = new List<Registration>();
        public IReadOnlyCollection<Registration> Registrations => _registrationList.AsReadOnly();
        public string FullName => $"{FirstName} {LastName}";
        public void UpdatePersonalInfo(string firstName, string lastName, string? phoneNumber)
        {
            FirstName = firstName ?? throw new DomainException($"Null у параметра:{nameof(firstName)}");
            LastName = lastName ?? throw new DomainException($"Null у параметра:{nameof(lastName)}");
            PhoneNumber = phoneNumber;
        }
        public void UpdateEmail(string newEmail)
        {
            Email = newEmail ?? throw new DomainException($"Null у параметра:{nameof(newEmail)}");
        }
        public void ChangePassword(string newPassword)
        {
            PasswordHash = newPassword ?? throw new DomainException($"Null у параметра:{nameof(newPassword)}");
        }
        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
        }
        public void AddRegistration(Registration newRegistration)
        {
            _registrationList.Add(newRegistration);
        }
        public void RemoveRegistration(Guid removableRegistrationGuid)
        {
            var registration = _registrationList.FirstOrDefault(reg => reg.Id == removableRegistrationGuid);
            if(registration == null) 
                throw new DomainException($"Подписки с id={removableRegistrationGuid} не существует");
            _registrationList.Remove(registration);
        }
        public void SetOrganizerProfile(Organizer newOrganizerProfile)
        {
            OrganizerProfile = newOrganizerProfile;
        }
        public void RemoveOrganizerProfile()
        {
            OrganizerProfile = null;
        }
    }
}