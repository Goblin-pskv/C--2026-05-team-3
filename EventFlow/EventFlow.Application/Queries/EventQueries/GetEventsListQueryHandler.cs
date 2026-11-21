using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;

namespace EventFlow.Application.Queries.EventQueries
{
    public class GetEventsListQueryHandler : IRequestHandler<GetEventsListQuery, Result<PageResponse<EventDto>>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GetEventsListQueryHandler> _logger;

        public GetEventsListQueryHandler(
            IEventRepository eventRepository,
            ILogger<GetEventsListQueryHandler> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<Result<PageResponse<EventDto>>> Handle(GetEventsListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение списка опубликованных мероприятий со страницей {Page} размером {PageSize}",
                request.Page, request.PageSize);

            try
            {
                var (events, totalCount) = await _eventRepository.GetPublishedEventsAsync(
                    request.Page,
                    request.PageSize,
                    cancellationToken);

                var dtos = events.Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Type = e.Type.ToString(),
                    Start = e.Start,
                    End = e.End,
                    City = e.City,
                    Address = e.Address,
                    Price = e.Price,
                    MaxParticipants = e.MaxParticipants,
                    IsPublished = e.IsPublished,
                    Status = e.Status.ToString(),
                    ImageUrl = e.ImageUrl,
                    CreatedAt = e.CreatedAt
                }).ToList();

                var enumTypes = Enum.GetValues<EventType>()
                    .ToDictionary(
                        e => e.ToString(),
                        e => GetDescription(e));

                var enumStatuses = Enum.GetValues<EventStatus>()
                    .ToDictionary(
                        e => e.ToString(),
                        e => GetDescription(e));

                _logger.LogInformation("Успешно получено {Count} мероприятий из {TotalCount}", dtos.Count, totalCount);
                return Result<PageResponse<EventDto>>.Success(new PageResponse<EventDto>
                {
                    Data = new PaginatedResult<EventDto>
                    {
                        Items = dtos,
                        TotalCount = totalCount
                    },
                    Dictionaries = new List<Dictionary<string, string>> { enumTypes, enumStatuses }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка мероприятий");
                return Result<PageResponse<EventDto>>.Failure("Произошла ошибка при получении списка мероприятий", 500);
            }
        }

        private static string GetDescription(Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var attribute = member?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
    }
}