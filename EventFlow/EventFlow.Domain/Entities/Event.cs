using EventFlow.Domain.Common;
using EventFlow.Domain.Enums;
using EventFlow.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Entities
{
    public class Event : BaseEntity
    {
        /// <summary>
        /// ID организатора, который создал мероприятие.
        /// Внешний ключ на таблицу Organizers.
        /// Определяет, кто может редактировать мероприятие.
        /// </summary>
        public Guid OrganizerId { get; set; }

        /// <summary>
        /// Название мероприятия        
        /// </summary>
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public EventType Type { get; set;  }
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }
        public string City { get; set; } = string.Empty;
        public string Address {  get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;

        public int? MaxParticipants { get; set; }

        /// <summary>
        /// Текущий статус мероприятия.
        /// Определяет, что можно делать с мероприятием.
        /// Пример: EventStatus.Published
        /// </summary>
        public EventStatus Status { get; set; } = EventStatus.Draft;
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Флаг публикации мероприятия.
        /// true = видно в каталоге, доступна регистрация
        /// false = черновик, видит только организатор
        /// Дублирует Status для быстрого фильтра
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Организатор мероприятия.
        /// Навигационное свойство для EF Core.
        /// Связь: одно мероприятие принадлежит одному Organizer (N:1)
        /// </summary>
        public virtual Organizer Organizer { get; set; } = null!;


        /// <summary>
        /// Список всех регистраций на это мероприятие.
        /// Показывает, кто записался на мероприятие.
        /// Связь: одно мероприятие может иметь много Registrations (1:N)
        /// </summary>
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        /// <summary>
        /// Проверяет, есть ли свободные места на мероприятии.
        /// Используется перед регистрацией пользователя.
        /// 
        /// Логика:
        /// - Если MaxParticipants = NULL → места всегда есть
        /// - Иначе считаем подтвержденные регистрации
        /// - Если confirmedCount < MaxParticipants → есть места
        /// </summary>
        /// <returns>true если есть свободные места</returns>
        /// <example>
        /// if (!event.HasAvailableSpots())
        ///     throw new DomainException("Нет свободных мест");
        /// </example>
        public bool HasAvailableSpots()
        {
            if (!MaxParticipants.HasValue)
                return true;

            var confirmedCount = Registrations.Count(r =>
                    r.Status == RegistrationStatus.Confirmed);

            return confirmedCount < MaxParticipants;
        }

        /// <summary>
        /// Возвращает количество свободных мест.
        /// Используется для отображения в UI.
        /// </summary>
        /// <returns>Количество свободных мест или int.MaxValue если без лимита</returns>
        public int GetAvailableSpotsCount()
        {
            if (!MaxParticipants.HasValue)
                return int.MaxValue; // Без ограничений

            var confirmedCount = Registrations.Count(r =>
                r.Status == RegistrationStatus.Confirmed);

            return MaxParticipants.Value - confirmedCount;
        }


        /// <summary>
        /// Публикует мероприятие (переводит в статус Published).
        /// Делает мероприятие видимым в каталоге.
        /// 
        /// Правила:
        /// - Можно публиковать только черновики (Draft)
        /// - Должны быть заполнены обязательные поля
        /// - Дата начала должна быть в будущем
        /// </summary>
        /// <exception cref="DomainException">
        /// Если мероприятие не в статусе Draft
        /// </exception>
        public void Publish()
        {
            if (Status != EventStatus.Draft)
                throw new DomainException("Можно публиковать только черновики");

            if (string.IsNullOrWhiteSpace(Title))
                throw new DomainException("Название обязательно");

            if (string.IsNullOrWhiteSpace(Address))
                throw new DomainException("Адрес обязателен");

            if (Start <= DateTime.UtcNow)
                throw new DomainException("Дата начала должна быть в будущем");

            Status = EventStatus.Published;
            IsPublished = true;

        }

        public void Canceled()
        {
            if (Status == EventStatus.Completed)
                throw new DomainException("Нельзя отменить завершенное мероприятие");

            Status = EventStatus.Cancelled;
            IsPublished = false;
        }

    }
}
