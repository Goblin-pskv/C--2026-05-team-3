using EventFlow.Application.Common;
using EventFlow.Domain.Enums;

namespace EventFlow.Application.DTOs
{
    /// <summary>
    /// DTO для ответа страницы с пагинированным списком и списком enum-значений.
    /// </summary>
    public class PageResponse<T>
    {
        public PaginatedResult<T> Data { get; set; } = new();
        public List<Dictionary<string, string>> Dictionaries { get; set; } = new();
    }
}