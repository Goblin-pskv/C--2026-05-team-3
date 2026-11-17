using EventFlow.Domain.Enums;

namespace EventFlow.Application.DTOs
{
    /// <summary>
    /// DTO для отображения заявки на роль организатора.
    /// Используется в запросах списка заявок (admin) и деталей заявки.
    /// </summary>
    public class OrganizerRequestDto
    {
        /// <summary>ID заявки</summary>
        public Guid Id { get; set; }

        /// <summary>ID пользователя, подавшего заявку</summary>
        public Guid UserId { get; set; }

        /// <summary>Имя пользователя для удобного просмотра администратором</summary>
        public string? UserName { get; set; }

        /// <summary>Статус рассмотрения заявки</summary>
        public OrganizerRequestStatus Status { get; set; }

        /// <summary>Данные заявки в формате JSON (CompanyName, Description, WebSite)</summary>
        public string? Data { get; set; }

        /// <summary>Причина отклонения (заполняется администратором)</summary>
        public string? RejectionReason { get; set; }

        /// <summary>Дата и время рассмотрения заявки</summary>
        public DateTime? ReviewedAt { get; set; }

        /// <summary>Дата создания заявки</summary>
        public DateTime CreatedAt { get; set; }
    }
}
