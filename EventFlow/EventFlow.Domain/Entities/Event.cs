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
        /// Конструктор для бизнес создания модели
        /// </summary>
        /// <param name="organizerId"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="type"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="city"></param>
        /// <param name="address"></param>
        /// <param name="price"></param>
        /// <param name="maxParticipants"></param>
        /// <param name="imageUrl"></param>
        /// <exception cref="DomainException"></exception>
        public Event(Guid organizerId, string title, string? description, EventType type,
             DateTime start, DateTime end, string city, string address,
             decimal price = 0, int? maxParticipants = null, string? imageUrl = null)
            : base(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow)
        {
            OrganizerId = organizerId;
            Title = title ?? throw new DomainException($"Null у параметра:{nameof(title)}");
            Description = description;
            Type = type;
            Start = start;
            End = end;
            City = city ?? throw new DomainException($"Null у параметра:{nameof(city)}");
            Address = address ?? throw new DomainException($"Null у параметра:{nameof(address)}");
            Price = price;
            MaxParticipants = maxParticipants;
            ImageUrl = imageUrl;
        }
        /// <summary>
        /// Конструктор для EF Core
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createdAt"></param>
        /// <param name="updatedAt"></param>
        /// <param name="organizerId"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="type"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="city"></param>
        /// <param name="address"></param>
        /// <param name="price"></param>
        /// <param name="maxParticipants"></param>
        /// <param name="status"></param>
        /// <param name="imageUrl"></param>
        /// <param name="isPublished"></param>
        /// <param name="organizer"></param>
        /// <param name="registrations"></param>
        private Event(Guid id, DateTime createdAt, DateTime updatedAt, Guid organizerId,
            string title, string? description, EventType type, DateTime start, 
            DateTime end, string city, string address, decimal price,
            int? maxParticipants, EventStatus status, string? imageUrl, bool isPublished,
            Organizer organizer, List<Registration> registrations) 
            : base(id,createdAt,updatedAt)
        {
            OrganizerId = organizerId;
            Title = title;
            Description = description;
            Type = type;
            Start = start;
            End = end;
            City = city;
            Address = address;
            Price = price;
            MaxParticipants = maxParticipants;
            Status = status;
            ImageUrl = imageUrl;
            IsPublished = isPublished;
            Organizer = organizer;
            _registrations = registrations ?? new List<Registration>();
        }
        /// <summary>
        /// ID организатора, который создал мероприятие.
        /// Внешний ключ на таблицу Organizers.
        /// Определяет, кто может редактировать мероприятие.
        /// </summary>
        public Guid OrganizerId { get; private set; }

        /// <summary>
        /// Название мероприятия        
        /// </summary>
        public string Title { get; private set; }
        public string? Description { get; private set; } 
        public EventType Type { get; private set;  }
        public DateTime Start {  get; private set; }
        public DateTime End { get; private set; }
        public string City { get; private set; }
        public string Address {  get; private set; }
        public decimal Price { get; private set; } = 0;
        /// <summary>
        /// Максимальное количество участников
        /// </summary>
        public int? MaxParticipants { get; private set; }
        /// <summary>
        /// Текущий статус мероприятия.
        /// Определяет, что можно делать с мероприятием.
        /// Пример: EventStatus.Published
        /// </summary>
        public EventStatus Status { get; private set; } = EventStatus.Draft;
        public string? ImageUrl { get; private set; }

        /// <summary>
        /// Флаг публикации мероприятия.
        /// true = видно в каталоге, доступна регистрация
        /// false = черновик, видит только организатор
        /// Дублирует Status для быстрого фильтра
        /// </summary>
        public bool IsPublished { get; private set; }

        /// <summary>
        /// Организатор мероприятия.
        /// Навигационное свойство для EF Core.
        /// Связь: одно мероприятие принадлежит одному Organizer (N:1)
        /// </summary>
        public Organizer Organizer { get; private set; }


        /// <summary>
        /// Список всех регистраций на это мероприятие.
        /// Показывает, кто записался на мероприятие.
        /// Связь: одно мероприятие может иметь много Registrations (1:N)
        /// </summary>
        private readonly List<Registration> _registrations = new List<Registration>();
        public IReadOnlyCollection<Registration> Registrations => _registrations.AsReadOnly();

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
        /// <summary>
        /// Отмена мероприятия
        /// </summary>
        /// <exception cref="DomainException"></exception>
        public void Cancel()
        {
            if (Status == EventStatus.Completed)
                throw new DomainException("Нельзя отменить завершенное мероприятие");

            Status = EventStatus.Cancelled;
            IsPublished = false;
        }
        /// <summary>
        /// Обновление основной информации мероприятия
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="city"></param>
        /// <param name="address"></param>
        /// <param name="price"></param>
        public void UpdateDetails(string title, string? description, string city, string address, decimal price)
        {
            Title = title ?? throw new DomainException($"Null у параметра:{nameof(title)}");
            Description = description;
            City = city ?? throw new DomainException($"Null у параметра:{nameof(city)}");
            Address = address ?? throw new DomainException($"Null у параметра:{nameof(address)}");
            Price = price;
        }
        /// <summary>
        /// Обновление времени мероприятия
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <exception cref="DomainException"></exception>
        public void UpdateDateTime(DateTime start, DateTime end)
        {
            if (end <= start) throw new DomainException("Окончание должно быть позже начала");
            Start = start;
            End = end;
        }
        /// <summary>
        /// Обновление количества регистраций
        /// </summary>
        /// <param name="maxParticipants"></param>
        /// <exception cref="DomainException"></exception>
        public void UpdateMaxParticipants(int? maxParticipants)
        {
            if (maxParticipants < Registrations.Count)
                throw new DomainException("Невозможно установить количество регистраций меньше текущего");
            MaxParticipants = maxParticipants;
        }
        /// <summary>
        /// Обновление изображения
        /// </summary>
        /// <param name="imageUrl"></param>
        public void UpdateImage(string? imageUrl)
        {
            ImageUrl = imageUrl;
        }
        public void SetOrganizer(Organizer organizer)
        {
            Organizer = organizer ?? throw new ArgumentNullException(nameof(organizer));
        }
    }
}