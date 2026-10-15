using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EventFlow.Application.Queries.OrganizerQueries
{
    public class GetAllOrganizerQueryHandler : IRequestHandler<GetAllOrganizerQuery, Result<IEnumerable<Organizer>>>
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetAllOrganizerQueryHandler> _logger;

        public GetAllOrganizerQueryHandler(IOrganizerRepository organizerRepository, IUserRepository userRepository, ILogger<GetAllOrganizerQueryHandler> logger)
        {
            _organizerRepository = organizerRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Organizer>>> Handle(GetAllOrganizerQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение всех заявок организаторов");
            
            try
            {
                var organizers = await _organizerRepository.GetPendingAsync(request.offset, request.count, ct);
                _logger.LogInformation("Заявки организаторов успешно получены. Количество: {Count}", organizers.Count());
                
                return Result<IEnumerable<Organizer>>.Success(organizers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех заявок организаторов");
                return Result<IEnumerable<Organizer>>.Failure("Ошибка при получении заявок организаторов", 500);
            }
        }
    }
}