using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;



namespace EventFlow.Application.Commands.EventCommands
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Result>
    {
        private readonly IValidator<CreateEventCommand> _validator;
        private readonly IEventRepository _eventRepository;
        private ILogger<CreateEventCommandHandler> _logger;


        public CreateEventCommandHandler(IEventRepository eventRepository,
                                           IValidator<CreateEventCommand> validator,
                                           ILogger<CreateEventCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _validator = validator;
            _logger = logger;
        }


        public async Task<Result> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                           "Регистрация начата для мероприятия: {Title})",
                           request.Title
                       );
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            var new_event = new Event
            {
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                Start = request.Start,
                End = request.End,
                City = request.City,
                Address = request.Address,
                Price = request.Price,
                MaxParticipants = request.MaxParticipants
            };

            try
            {
                await _eventRepository.AddAsync(new_event);
                await _eventRepository.SaveChangesAsync();

                _logger.LogInformation("Мероприятие {Title} успешно создано", request.Title);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "Ошибка при регистрации для {Title}: {Error}",
                    request.Title,
                    ex.Message
                );

                return Result.Failure($"Ошибка создания мероприятия: {ex.Message}", 500);
            }
        }
    }
}