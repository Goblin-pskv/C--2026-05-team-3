namespace EventFlow.Domain.Enums
{
    /// <summary>
    /// Статус заявки на роль организатора.
    /// </summary>
    public enum OrganizerRequestStatus
    {
        /// <summary>
        /// Заявка создана, ожидает рассмотрения администратором.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Заявка одобрена администратором. Пользователю назначена роль Organizer.
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Заявка отклонена администратором.
        /// </summary>
        Rejected = 2
    }
}
