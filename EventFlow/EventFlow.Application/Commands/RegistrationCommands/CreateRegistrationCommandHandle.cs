using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.RegistrationCommands
{
    public class CreateRegistrationCommandHandle : IRequestHandler<CreateRegistrationCommand, Result<Guid>>
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateRegistrationCommand> _validator;
        private readonly ILogger<CreateRegistrationCommandHandle> _logger;

        public CreateRegistrationCommandHandle(
           IRegistrationRepository registrationRepository,
           IEventRepository eventRepository,
           ITicketRepository ticketRepository,
           IUnitOfWork unitOfWork,
           IValidator<CreateRegistrationCommand> validator,
           ILogger<CreateRegistrationCommandHandle> logger)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreateRegistrationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Регистрация пользователя {UserId} на мероприятие {EventId}",
                request.UserId, request.EventId);

            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Ошибка валидации регистрации: {Errors}", errors);
                return Result<Guid>.Failure(errors, 400);
            }

            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (@event is null)
            {
                _logger.LogWarning("Мероприятие {EventId} не найдено", request.EventId);
                return Result<Guid>.Failure($"Мероприятие с Id {request.EventId} не найдено", 404);
            }

            if (!@event.IsPublished)
            {
                _logger.LogWarning("Мероприятие {EventId} не опубликовано", request.EventId);
                return Result<Guid>.Failure("Нельзя зарегистрироваться на неопубликованное мероприятие", 422);
            }

            if (@event.Start <= DateTime.UtcNow)
            {
                _logger.LogWarning("Мероприятие {EventId} уже началось", request.EventId);
                return Result<Guid>.Failure("Нельзя зарегистрироваться на начавшееся мероприятие", 422);
            }

            var existing = await _registrationRepository
                .GetRegistrationAsync(request.UserId, request.EventId, cancellationToken);

            if (existing is not null)
            {
                if (existing.Status == RegistrationStatus.Cancelled)
                    return await RestoreRegistrationAndTicketAsync(request, @event, cancellationToken);

                _logger.LogWarning(
                    "Пользователь {UserId} уже зарегистрирован на {EventId} (статус {Status})",
                    request.UserId, request.EventId, existing.Status);
                return Result<Guid>.Failure("Вы уже зарегистрированы на это мероприятие", 409);
            }

            if (@event.MaxParticipants.HasValue)
            {
                var currentCount = await _ticketRepository
                    .CountActiveAsync(request.EventId, cancellationToken);

                if (currentCount >= @event.MaxParticipants.Value)
                {
                    _logger.LogWarning(
                        "Мероприятие {EventId} заполнено: {Current}/{Max}",
                        request.EventId, currentCount, @event.MaxParticipants.Value);
                    return Result<Guid>.Failure("Достигнут лимит участников", 422);
                }
            }

            return await SaveRegistrationAndTicketAsync(request, @event, cancellationToken);
        }

        private async Task<Result<Guid>> SaveRegistrationAndTicketAsync(
            CreateRegistrationCommand request, Event @event, CancellationToken ct)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(ct);

                var registration = new Registration
                {
                    Id = Guid.NewGuid(),
                    EventId = request.EventId,
                    UserId = request.UserId,
                    Status = RegistrationStatus.Confirmed,
                    RegistrationDate = DateTime.UtcNow,
                    ConfirmationDate = DateTime.UtcNow
                };
                await _registrationRepository.AddAsync(registration, ct);

                var ticketCount = (await _ticketRepository.GetByEventAsync(request.EventId, ct)).Count;
                var ticketNumber = $"EVT-{@event.Start.Year}-{ticketCount + 1:D6}";
                var ticket = new Tickets
                {
                    Id = Guid.NewGuid(),
                    EventId = request.EventId,
                    UserId = request.UserId,
                    TicketNumber = ticketNumber,
                    Price = @event.Price,
                    status = TicketStatus.Pending,
                    TicketCreationDate = DateTime.UtcNow
                };
                await _ticketRepository.AddAsync(ticket, ct);

                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitAsync(ct);

                _logger.LogInformation(
                    "Пользователь {UserId} успешно зарегистрирован на {EventId}, билет {TicketNumber}",
                    request.UserId, request.EventId, ticketNumber);

                return Result<Guid>.Success(ticket.Id);
            }
            catch (DbUpdateException dbEx)
            {
                try { await _unitOfWork.RollbackAsync(ct); } catch { }
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "Ошибка сохранения регистрации: {Inner}", inner);
                return Result<Guid>.Failure("Не удалось создать регистрацию. Возможно, вы уже записаны.", 409);
            }
            catch (Exception ex)
            {
                try { await _unitOfWork.RollbackAsync(ct); } catch { }
                _logger.LogError(ex,
                    "Ошибка при регистрации пользователя {UserId} на {EventId}: {Error}",
                    request.UserId, request.EventId, ex.Message);
                return Result<Guid>.Failure($"Ошибка регистрации: {ex.Message}", 500);
            }
        }

        private async Task<Result<Guid>> RestoreRegistrationAndTicketAsync(
            CreateRegistrationCommand request, Event @event, CancellationToken ct)
        {
            _logger.LogInformation(
                "Повторная регистрация пользователя {UserId} на {EventId}",
                request.UserId, request.EventId);

            try
            {
                await _unitOfWork.BeginTransactionAsync(ct);

                var existing = await _registrationRepository
                    .GetRegistrationAsync(request.UserId, request.EventId, ct);
                existing!.Status = RegistrationStatus.Confirmed;
                existing.ConfirmationDate = DateTime.UtcNow;
                await _registrationRepository.UpdateAsync(existing, ct);

                var tickets = await _ticketRepository.GetByEventAsync(request.EventId, ct);
                var existingTicket = tickets.FirstOrDefault(t => t.UserId == request.UserId);
                Guid ticketId;
                if (existingTicket != null)
                {
                    ticketId = existingTicket.Id;
                    if (existingTicket.status == TicketStatus.Cancelled)
                    {
                        existingTicket.status = TicketStatus.Pending;
                        await _ticketRepository.UpdateAsync(existingTicket, ct);
                    }
                }
                else
                {
                    var ticketNumber = $"EVT-{@event.Start.Year}-{tickets.Count + 1:D6}";
                    var ticket = new Tickets
                    {
                        Id = Guid.NewGuid(),
                        EventId = request.EventId,
                        UserId = request.UserId,
                        TicketNumber = ticketNumber,
                        Price = @event.Price,
                        status = TicketStatus.Pending,
                        TicketCreationDate = DateTime.UtcNow
                    };
                    await _ticketRepository.AddAsync(ticket, ct);
                    ticketId = ticket.Id;
                }

                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitAsync(ct);
                return Result<Guid>.Success(ticketId);
            }
            catch (Exception ex)
            {
                try { await _unitOfWork.RollbackAsync(ct); } catch { }
                _logger.LogError(ex,
                    "Ошибка при восстановлении регистрации пользователя {UserId} на {EventId}",
                    request.UserId, request.EventId);
                return Result<Guid>.Failure($"Ошибка восстановления: {ex.Message}", 500);
            }
        }
    }
}
