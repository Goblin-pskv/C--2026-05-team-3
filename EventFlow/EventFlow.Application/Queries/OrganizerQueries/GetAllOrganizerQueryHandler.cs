using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Queries.OrganizerQueries
{
    public class GetAllOrganizerQueryHandler : IRequestHandler<GetAllOrganizerQuery, Result<IEnumerable<OrganizerRequestDto>>>
    {
        private readonly IOrganizerRequestRepository _organizerRequestRepository;
        private readonly ILogger<GetAllOrganizerQueryHandler> _logger;

        public GetAllOrganizerQueryHandler(IOrganizerRequestRepository organizerRequestRepository, ILogger<GetAllOrganizerQueryHandler> logger)
        {
            _organizerRequestRepository = organizerRequestRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<OrganizerRequestDto>>> Handle(GetAllOrganizerQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Получение всех заявок организаторов");

            try
            {
                var requests = await _organizerRequestRepository.GetPendingAsync(request.offset, request.count, ct);
                var dtoList = requests.Select(r => new OrganizerRequestDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = $"{r.User.FirstName} {r.User.LastName}".Trim(),
                    Status = r.Status,
                    Data = r.Data,
                    RejectionReason = r.RejectionReason,
                    ReviewedAt = r.ReviewedAt,
                    CreatedAt = r.CreatedAt
                });

                _logger.LogInformation("Заявки организаторов успешно получены.");

                return Result<IEnumerable<OrganizerRequestDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех заявок организаторов");
                return Result<IEnumerable<OrganizerRequestDto>>.Failure("Ошибка при получении заявок организаторов", 500);
            }
        }
    }
}