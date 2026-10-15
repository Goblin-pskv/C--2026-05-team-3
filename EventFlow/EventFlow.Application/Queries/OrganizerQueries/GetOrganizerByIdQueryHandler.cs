using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.OrganizerQueries
{
    public class GetOrganizerByIdQueryHandler : IRequestHandler<GetOrganizerByIdQuery, Result<OrganizerProfileRequestDto>>
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetOrganizerByIdQueryHandler> _logger;

        public GetOrganizerByIdQueryHandler(IOrganizerRepository organizerRepository, IUserRepository userRepository, ILogger<GetOrganizerByIdQueryHandler> logger)
        {
            _organizerRepository = organizerRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Result<OrganizerProfileRequestDto>> Handle(GetOrganizerByIdQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение заявки организатора по ID: {Id}", request.Id);
            
            try
            {
                var organizer = await _organizerRepository.GetByIdAsync(request.Id, ct);
                if (organizer == null)
                {
                    _logger.LogWarning("Заявка организатора с ID {Id} не найдена", request.Id);
                    return Result<OrganizerProfileRequestDto>.Failure("Заявка организатора не найдена", 404);
                }
                var dto = new OrganizerProfileRequestDto()
                {
                    Id = organizer.Id,
                    CompanyName = organizer.CompanyName,
                    Description = organizer.Description,
                    WebSite = organizer.WebSite
                };
                _logger.LogInformation("Заявка организатора с ID {Id} успешно получена", request.Id);
                return Result<OrganizerProfileRequestDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении заявки организатора по ID: {Id}", request.Id);
                return Result<OrganizerProfileRequestDto>.Failure("Ошибка при получении заявки организатора", 500);
            }
        }
    }
}