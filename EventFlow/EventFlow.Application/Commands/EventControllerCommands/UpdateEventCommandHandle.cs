
using EventFlow.API.Controllers;
using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventFlow.Application.Commands.EventCommands
{
    public class UpdateEventCommandHandle : IRequestHandler<UpdateEventCommand, Result>
    {

        private readonly IValidator<UpdateEventCommand> _validator;
        private readonly IEventRepository _eventRepository;
        private ILogger<UpdateEventCommandHandle> _logger;


        public UpdateEventCommandHandle(IEventRepository eventRepository,
                                           IValidator<UpdateEventCommand> validator,
                                           ILogger<UpdateEventCommandHandle> logger)
        {
            _eventRepository = eventRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Обновление мероприятия начато: {Id} — {Title}", request.EventId, request.Title);

            //Валидация
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Ошибка валидации при обновлении {Id}: {Errors}", request.EventId, errors);
                return Result.Failure(errors, 400);
            }

            //Поиск существующего мероприятия
            var existingEvent = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (existingEvent == null)
            {
                _logger.LogWarning("Мероприятие {Id} не найдено", request.EventId);
                return Result.Failure($"Мероприятие с Id {request.EventId} не найдено", 404);
            }

            //Обновление полей
            existingEvent.Title = request.Title;
            existingEvent.Description = request.Description;
            existingEvent.Type = request.Type;
            existingEvent.Start = request.Start;
            existingEvent.End = request.End;
            existingEvent.City = request.City;
            existingEvent.Address = request.Address;
            existingEvent.Price = request.Price;
            existingEvent.MaxParticipants = request.MaxParticipants;
            existingEvent.OrganizerId = request.OrganizerId;
            existingEvent.IsPublished = request.IsPublished;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            try
            {
                _eventRepository.UpdateAsync(existingEvent, cancellationToken);
                await _eventRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Мероприятие {Title} (Id: {Id}) успешно обновлено", request.Title, request.EventId);
                return Result.Success();
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                _logger.LogError(concEx, "Конфликт параллельного обновления мероприятия {Id}", request.EventId);
                return Result.Failure("Мероприятие было изменено другим пользователем. Попробуйте снова.", 409);
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "Ошибка сохранения при обновлении: {Inner}", inner);
                return Result.Failure($"Ошибка сохранения: {inner}", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении мероприятия {Id}: {Error}", request.EventId, ex.Message);
                return Result.Failure($"Ошибка обновления мероприятия: {ex.Message}", 500);
            }
        }
    }
}