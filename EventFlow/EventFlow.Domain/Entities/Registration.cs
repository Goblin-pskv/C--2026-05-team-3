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
    internal class Registration : BaseEntity
    {
        /// <summary>
        /// ID мероприятия, на которое регистрируется пользователь.
        /// Внешний ключ на таблицу Events.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// ID пользователя, который регистрируется.
        /// Внешний ключ на таблицу Users.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Статус регистрации.
        /// Определяет, на каком этапе находится запись.
        /// </summary>
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Confirmed;

        /// <summary>
        /// Дата и время создания регистрации (в UTC).
        /// Заполняется автоматически при создании.      
        /// </summary>
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;


        /// <summary>
        /// Дата подтверждения регистрации (в UTC).
        /// NULL если еще не подтверждена.
        /// Заполняется при переходе в статус Confirmed.        
        /// </summary>
        public DateTime? ConfirmationDate { get; set; }


        /// <summary>
        /// Мероприятие, на которое регистрируется пользователь.
        /// Навигационное свойство для EF Core.
        /// Связь: одна Registration принадлежит одному Event (N:1)
        /// </summary>
        public virtual Event Event { get; set; } = null!;

        public virtual User User { get; set; } = null!;


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
