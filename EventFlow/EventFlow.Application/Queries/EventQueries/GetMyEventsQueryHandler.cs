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
    public class GetMyEventsQueryHandler : IRequestHandler<GetMyEventsQuery, Result<PageResponse<EventDto>>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IOrganizerRepository _organizerRepository;
        private readonly ILogger<GetMyEventsQueryHandler> _logger;

        public GetMyEventsQueryHandler(IEventRepository eventRepository,
                                        IOrganizerRepository organizerRepository,
                                        ILogger<GetMyEventsQueryHandler> logger)
        {
            _eventRepository = eventRepository;
            _organizerRepository = organizerRepository;
            _logger = logger;
        }

        public async Task<Result<PageResponse<EventDto>>> Handle(GetMyEventsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение списка мероприятий организатора {OrganizerId} со страницей {Page} размером {PageSize}",
                request.OrganizerId, request.Page, request.PageSize);

            try
            {
                var organizer = await _organizerRepository.GetByUserIdAsync(request.OrganizerId, cancellationToken);
                if (organizer == null)
                {
                    _logger.LogWarning("Организатор не найден для UserId: {OrganizerId}", request.OrganizerId);
                    return Result<PageResponse<EventDto>>.Failure("Пользователь не является организатором", 404);
                }

                var (events, totalCount) = await _eventRepository.GetEventsByOrganizerWithPaginationAsync(
                    organizer.Id, request.Page, request.PageSize, cancellationToken);

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
                    ImageUrl = e.ImageUrl,
                    Status = e.Status.ToString(),
                    CreatedAt = e.CreatedAt
                }).ToList();

                var result = new PaginatedResult<EventDto>
                {
                    Items = dtos,
                    TotalCount = totalCount
                };

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
                    Data = result,
                    Dictionaries = new List<Dictionary<string, string>> { enumTypes, enumStatuses }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка мероприятий организатора {OrganizerId}", request.OrganizerId);
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
