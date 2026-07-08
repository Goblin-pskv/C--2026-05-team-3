using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Exceptions
{
    /// <summary>
    /// Исключение для нарушений бизнес-правил.
    /// Используется, когда операция невозможна из-за бизнес-логики.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Создает исключение с сообщением о нарушении бизнес-правила.
        /// </summary>
        /// <param name="message">Понятное описание проблемы для пользователя</param>
        /// <example>
        /// throw new DomainException("На мероприятии нет свободных мест");
        /// throw new DomainException("Нельзя отменить завершенное мероприятие");
        /// </example>
        public DomainException(string message) : base(message) { }

        /// <summary>
        /// Создает исключение с сообщением и внутренним исключением
        /// когда нужно сохранить оригинальную ошибку.
        /// </summary>
        /// <param name="message">Сообщение о нарушении бизнес-правила</param>
        /// <param name="innerException">Оригинальное исключение</param>
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
