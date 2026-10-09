using EventFlow.Application.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<EmailMessageService> _loger;

        public EmailMessageService(IOptions<EmailServiceSettings> emailSettings, ILogger<EmailMessageService> logger)
        {
            _settings = emailSettings.Value;
            _loger = logger;
        }

        /// <summary>
        /// Отправить email сообщение.
        /// </summary>
        /// <param name="to">Адрес получателя</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="body">Тело письма</param>
        /// <returns>Завершение операции</returns>
        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken ct)
        {
            if (_settings.SmtpHost == string.Empty)
                return;
            using var client = GetSmtpClient();
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            message.To.Add(to);
            try
            {
                await client.SendMailAsync(message);
                _loger.LogInformation("Отправленно сообщение на адрес {to}",to);
            }
            catch(Exception ex)
            {
                _loger.LogError("Ошибка при отправке Email сообщения({to}). Сообщение ошибки: {Message}", to, ex.Message);
            }
        }

        /// <summary>
        /// Отправить email сообщение с возможностью указать список адресов копии.
        /// </summary>
        /// <param name="to">Адрес получателя</param>
        /// <param name="cc">Список адресов копии</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="body">Тело письма</param>
        /// <returns>Завершение операции</returns>
        public async Task SendEmailAsync(string to, IEnumerable<string> cc, string subject, string body, CancellationToken ct)
        {
            if (_settings.SmtpHost == string.Empty)
                return;
            using var client = GetSmtpClient();
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            foreach (var address in cc)
            {
                message.CC.Add(address);
            }
            try
            {
                await client.SendMailAsync(message);
                _loger.LogInformation("Отправленно сообщение на адрес {to} и несколько других в копиях", to);
            }
            catch(Exception ex)
            {
                _loger.LogError("Ошибка при отправке Email сообщении. Сообщение ошибки: {Message}.", ex.Message);
            }
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
        public async Task SendEmailAsync(string to, IEnumerable<string> cc, IEnumerable<string> bcc, string subject, string body, CancellationToken ct)
        {
            if (_settings.SmtpHost == string.Empty)
                return;
            using var client = GetSmtpClient();
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            foreach (var address in cc)
            {
                message.CC.Add(address);
            }

            foreach (var address in bcc)
            {
                message.Bcc.Add(address);
            }
            try
            {
                await client.SendMailAsync(message);
                _loger.LogInformation("Отправленно сообщение на адрес {to} и несколько других в копиях. В том числе скрытых", to);
            }
            catch(Exception ex)
            {
                _loger.LogError("Ошибка при отправке Email сообщения. Сообщение ошибки: {Message}", ex.Message);
            }
        }

        private SmtpClient GetSmtpClient()
        {
            return new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password),
                EnableSsl = false
            };
        }
    }
}