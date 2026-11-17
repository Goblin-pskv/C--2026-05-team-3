using EventFlow.Domain.Common;

namespace EventFlow.Domain.Entities
{
    /// <summary>
    /// Заявка пользователя на роль организатора.
    /// Отдельная сущность от профиля Organizer — хранит только статус рассмотрения заявки.
    ///
    /// Жизненный цикл:
    /// 1. Пользователь подает заявку → Status = Pending
    /// 2. Администратор рассматривает
    ///    - Одобрение → Status = Approved, назначается роль Organizer
    ///    - Отклонение → Status = Rejected, сохраняется причина
    /// </summary>
    public class OrganizerRequest : BaseEntity
    {
        /// <summary>
        /// ID пользователя, подавшего заявку.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Статус рассмотрения заявки.
        /// </summary>
        public Domain.Enums.OrganizerRequestStatus Status { get; set; } = Domain.Enums.OrganizerRequestStatus.Pending;

        /// <summary>
        /// Данные заявки в формате JSON (CompanyName, Description, WebSite).
        /// Используется администратором при рассмотрении.
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// Причина отклонения заявки (заполняется администратором при отклонении).
        /// </summary>
        public string? RejectionReason { get; set; }

        /// <summary>
        /// ID администратора, который рассмотрел заявку.
        /// </summary>
        public Guid? ReviewedById { get; set; }

        /// <summary>
        /// Дата и время рассмотрения заявки (одобрения или отклонения).
        /// </summary>
        public DateTime? ReviewedAt { get; set; }

        /// <summary>
        /// Пользователь, подавший заявку.
        /// </summary>
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// Администратор, рассмотревший заявку.
        /// </summary>
        public virtual User Approver { get; set; } = null!;
    }
}
