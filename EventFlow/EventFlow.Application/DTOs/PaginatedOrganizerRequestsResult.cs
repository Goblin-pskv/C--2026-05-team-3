namespace EventFlow.Application.DTOs
{
    /// <summary>
    /// Ответ с пагинированным списком заявок и общим количеством.
    /// </summary>
    public class PaginatedOrganizerRequestsResult
    {
        public IEnumerable<OrganizerRequestDto> Items { get; set; } = Enumerable.Empty<OrganizerRequestDto>();
        public int TotalCount { get; set; }
    }
}
