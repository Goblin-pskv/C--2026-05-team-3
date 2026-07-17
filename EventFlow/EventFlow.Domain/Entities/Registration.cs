using EventFlow.Domain.Common;
using EventFlow.Domain.Enums;
using EventFlow.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Entities
{
    /// <summary>
    /// Регистрация пользователя на мероприятие.
    /// Связывает User и Event — кто записался на что.
    /// 
    /// Жизненный цикл:
    /// 1. Пользователь нажимает "Зарегистрироваться"
    /// 2. Создается Registration (Status = Pending или Confirmed)
    /// 3. Организатор подтверждает (Status = Confirmed)
    /// 4. Пользователь приходит (Status = Attended)
    /// 
    /// Связи:
    /// - Одна Registration принадлежит одному Event (N:1)
    /// - Одна Registration принадлежит одному User (N:1)
    /// - Уникальное ограничение: один User = одна Registration на Event
    /// </summary>
    public class Registration : BaseEntity
    {
        /// <summary>
        /// Конструктор для бизнес создания модели
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="eventReg"></param>
        /// <param name="user"></param>
        /// <exception cref="DomainException"></exception>
        public Registration(Guid eventId, Guid userId, Event @event, User user) 
            : base(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow)
        {
            EventId = eventId;
            UserId = userId;
            Status = RegistrationStatus.Pending;
            RegistrationDate = DateTime.UtcNow;
            ConfirmationDate = null;
            Event = @event ?? throw new DomainException($"Null у параметра {nameof(@event)}");
            User = user ?? throw new DomainException($"Null у параметра {nameof(user)}");
        }
        /// <summary>
        /// Конструктор для EF Core
        /// </summary>
        /// <param name="registrationId"></param>
        /// <param name="createdAt"></param>
        /// <param name="updatedAt"></param>
        /// <param name="eventId"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="registrationDate"></param>
        /// <param name="confirmationDate"></param>
        /// <param name="event"></param>
        /// <param name="user"></param>
        private Registration(Guid registrationId, DateTime createdAt, DateTime updatedAt, Guid eventId, 
            Guid userId, RegistrationStatus status, DateTime registrationDate, DateTime? confirmationDate, 
            Event @event, User user) 
            : base(registrationId, createdAt, updatedAt)
        {
            EventId = eventId;
            UserId = userId;
            Status = status;
            RegistrationDate = registrationDate;
            ConfirmationDate = confirmationDate;
            Event = @event;
            User = user;
        }

        /// <summary>
        /// ID мероприятия, на которое регистрируется пользователь.
        /// Внешний ключ на таблицу Events.
        /// </summary>
        public Guid EventId { get; private set; }

        /// <summary>
        /// ID пользователя, который регистрируется.
        /// Внешний ключ на таблицу Users.
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Статус регистрации.
        /// Определяет, на каком этапе находится запись.
        /// </summary>
        public RegistrationStatus Status { get; private set; }

        /// <summary>
        /// Дата и время создания регистрации (в UTC).
        /// Заполняется автоматически при создании.      
        /// </summary>
        public DateTime RegistrationDate { get; private set; }


        /// <summary>
        /// Дата подтверждения регистрации (в UTC).
        /// NULL если еще не подтверждена.
        /// Заполняется при переходе в статус Confirmed.        
        /// </summary>
        public DateTime? ConfirmationDate { get; private set; }


        /// <summary>
        /// Мероприятие, на которое регистрируется пользователь.
        /// Навигационное свойство для EF Core.
        /// Связь: одна Registration принадлежит одному Event (N:1)
        /// </summary>
        public Event Event { get; private set; }

        public User User { get; private set; }


        /// <summary>
        /// Подтверждает регистрацию (переводит в статус Confirmed).
        /// Используется организатором или автоматически.
        /// </summary>
        /// <exception cref="DomainException">
        /// Если регистрация не в статусе Pending
        /// </exception>
        public void Confirm()
        {
            if (Status != RegistrationStatus.Pending)
                throw new DomainException("Можно подтвердить только ожидающую регистрацию");

            Status = RegistrationStatus.Confirmed;
            ConfirmationDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Отменяет регистрацию (переводит в статус Cancelled).
        /// Используется пользователем или организатором.      
        /// </summary>
        public void Cancel()
        {
            if (Status == RegistrationStatus.Attended)
                throw new DomainException("Нельзя отменить посещенное мероприятие");

            Status = RegistrationStatus.Cancelled;         
        }

        public void MardAsAttended()
        {
            if (Status != RegistrationStatus.Confirmed)
                throw new DomainException("Можно отметить только подтвержденную регистрацию");

            Status = RegistrationStatus.Attended;
        }


    }
}
