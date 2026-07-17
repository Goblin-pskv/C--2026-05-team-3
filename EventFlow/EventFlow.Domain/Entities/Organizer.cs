using EventFlow.Domain.Common;
using EventFlow.Domain.Exceptions;
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
        /// Конструктор для бизнес создания модели
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyName"></param>
        /// <param name="description"></param>
        /// <param name="webSite"></param>
        /// <param name="isVerified"></param>
        /// <param name="user"></param>
        public Organizer(Guid userId, string? companyName, string? description, string? webSite,
            bool isVerified, User user) 
            : base(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow)
        {
            UserId = userId;
            CompanyName = companyName;
            Description = description;
            WebSite = webSite;
            IsVerified = isVerified;
            User = user ?? throw new DomainException($"Null у параметра:{nameof(user)}"); ;
            _events = new List<Event>();
        }
        /// <summary>
        /// Конструктор для EF Core
        /// </summary>
        /// <param name="organizerId"></param>
        /// <param name="createdAt"></param>
        /// <param name="updatedAt"></param>
        /// <param name="userId"></param>
        /// <param name="companyName"></param>
        /// <param name="description"></param>
        /// <param name="webSite"></param>
        /// <param name="isVerified"></param>
        /// <param name="user"></param>
        /// <param name="events"></param>
        private Organizer(Guid organizerId, DateTime createdAt, DateTime updatedAt, Guid userId, string? companyName, string? description, string? webSite,
            bool isVerified, User user, List<Event> events)
            : base(organizerId, createdAt, updatedAt)
        {
            UserId = userId;
            CompanyName = companyName;
            Description = description;
            WebSite = webSite;
            IsVerified = isVerified;
            this.User = user;
            _events = events ?? new List<Event>();
        }

        /// <summary>
        /// ID пользователя, которому принадлежит этот профиль.
        /// Связывает Organizer с User (внешний ключ).\
        /// </summary>
        public Guid UserId { get; private set; }

        public string? CompanyName { get; private set; }

        /// <summary>
        /// Описание организатора.
        /// </summary>
        public string? Description { get; private set; }
        public string? WebSite {  get; private set; }

        /// <summary>
        /// Флаг верификации организатора.
        /// true = документы проверены администрацией
        /// false = еще не проверен или отклонен
        /// </summary>
        public bool IsVerified { get; private set; }

        /// <summary>
        /// Пользователь, которому принадлежит этот профиль.
        /// Навигационное свойство для EF Core.
        /// Связь: один Organizer принадлежит одному User (1:1)
        /// </summary>
        public User User { get; private set; }
        /// <summary>
        /// Список всех мероприятий, созданных этим организатором.
        /// Показывает, какие мероприятия организовал этот человек/компания.
        /// Связь: один Organizer может создать много Events (1:N)
        /// </summary>
        private readonly List<Event> _events = new List<Event>();
        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();
        public void UpdateDetails(string? companyName, string? description, string? webSite)
        {
            CompanyName = companyName;
            Description = description;
            WebSite = webSite;
        }
        public void UpdateUser(User user)
        {
            User = user ?? throw new DomainException($"Null у параметра:{nameof(user)}");
            UserId = user.Id;
        }
        public void Verify()
        {
            IsVerified = true;
        }
        public void Unverify()
        {
            IsVerified = false;
        }
        public void AddEvent(Event @event)
        {
            if (@event == null) throw new DomainException(nameof(@event));
            _events.Add(@event);
        }
        public void RemoveEvent(Event @event)
        {
            if (@event == null) throw new DomainException(nameof(@event));
            if (!_events.Contains(@event))
                throw new DomainException("Мероприятие не найдено");
            _events.Remove(@event);
        }
    }
}