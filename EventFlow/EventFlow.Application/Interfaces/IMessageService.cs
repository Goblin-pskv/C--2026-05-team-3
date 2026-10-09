using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    /// <summary>
    /// Контракт для отправки сообщений (на данный момент только email).
    /// 
    /// Этот интерфейс определяет операции, которые должен уметь выполнять
    /// сервис отправки сообщений. Реализация находится в Infrastructure слое
    /// и может использовать различные способы отправки (SMTP, сторонние сервисы и т.д.)
    /// 
    /// Зачем нужен интерфейс:
    /// 1. Разделение абстракции и реализации (принцип Dependency Inversion)
    /// 2. Возможность подменить реализацию для unit-тестов (mock)
    /// 3. Application слой не зависит от конкретной технологии отправки
    /// 4. Легко заменить SMTP на другую систему отправки без изменения бизнес-логики
    /// 
    /// Как использовать:
    /// - Внедрите через DI: public SomeService(IMessageService messageService)
    /// - Все методы асинхронные для производительности
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Отправить email сообщение.
        /// </summary>
        /// <param name="to">Адрес получателя</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="body">Тело письма</param>
        /// <returns>Завершение операции</returns>
        Task SendEmailAsync(string to, string subject, string body, CancellationToken ct);

        /// <summary>
        /// Отправить email сообщение с возможностью указать список адресов копии.
        /// </summary>
        /// <param name="to">Адрес получателя</param>
        /// <param name="cc">Список адресов копии</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="body">Тело письма</param>
        /// <returns>Завершение операции</returns>
        Task SendEmailAsync(string to, IEnumerable<string> cc, string subject, string body, CancellationToken ct);

        /// <summary>
        /// Отправить email сообщение с возможностью указать список адресов копии и скрытой копии.
        /// </summary>
        /// <param name="to">Адрес получателя</param>
        /// <param name="cc">Список адресов копии</param>
        /// <param name="bcc">Список адресов скрытой копии</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="body">Тело письма</param>
        /// <returns>Завершение операции</returns>
        Task SendEmailAsync(string to, IEnumerable<string> cc, IEnumerable<string> bcc, string subject, string body, CancellationToken ct);
    }
}