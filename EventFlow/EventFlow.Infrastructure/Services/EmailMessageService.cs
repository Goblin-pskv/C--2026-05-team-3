using EventFlow.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EventFlow.Infrastructure.Services
{
    /// <summary>
    /// Реализация сервиса отправки сообщений через SMTP.
    /// </summary>
    public class EmailMessageService : IMessageService
    {
        private readonly EmailServiceSettings _settings;

        public EmailMessageService(IOptions<EmailServiceSettings> emailSettings)
        {
            _settings = emailSettings.Value;
        }

        /// <summary>
        /// Отправить email сообщение.
        /// </summary>
        /// <param name="to">Адрес получателя</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="body">Тело письма</param>
        /// <returns>Завершение операции</returns>
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (_settings.SmtpHost == string.Empty)
                return;
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password),
                EnableSsl = false
            };
            var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            message.To.Add(to);
            await client.SendMailAsync(message);
        }

        /// <summary>
        /// Отправить email сообщение с возможностью указать список адресов копии.
        /// </summary>
        /// <param name="to">Адрес получателя</param>
        /// <param name="cc">Список адресов копии</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="body">Тело письма</param>
        /// <returns>Завершение операции</returns>
        public async Task SendEmailAsync(string to, IEnumerable<string> cc, string subject, string body)
        {
            //var message = new MailMessage
            //{
            //    From = new MailAddress(_fromEmail, _displayName),
            //    Subject = subject,
            //    Body = body,
            //    IsBodyHtml = true
            //};
            //message.To.Add(to);
            
            //foreach (var address in cc)
            //{
            //    message.CC.Add(address);
            //}

            //await _smtpClient.SendMailAsync(message);
        }

        /// <summary>
        /// Отправить email сообщение с возможностью указать список адресов копии и скрытой копии.
        /// </summary>
        /// <param name="to">Адрес получателя</param>
        /// <param name="cc">Список адресов копии</param>
        /// <param name="bcc">Список адресов скрытой копии</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="body">Тело письма</param>
        /// <returns>Завершение операции</returns>
        public async Task SendEmailAsync(string to, IEnumerable<string> cc, IEnumerable<string> bcc, string subject, string body)
        {
            //var message = new MailMessage
            //{
            //    From = new MailAddress(_fromEmail, _displayName),
            //    Subject = subject,
            //    Body = body,
            //    IsBodyHtml = true
            //};
            //message.To.Add(to);
            
            //foreach (var address in cc)
            //{
            //    message.CC.Add(address);
            //}

            //foreach (var address in bcc)
            //{
            //    message.Bcc.Add(address);
            //}

            //await _smtpClient.SendMailAsync(message);
        }
    }
}