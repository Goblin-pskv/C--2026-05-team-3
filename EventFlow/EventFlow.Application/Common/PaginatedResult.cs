namespace EventFlow.Application.Common
{
    /// <summary>
    /// Универсальный ответ с пагинированным списком и общим количеством записей.
    /// </summary>
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
    }
}
