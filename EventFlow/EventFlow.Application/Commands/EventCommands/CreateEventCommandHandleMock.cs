using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.EventCommands
{
    public class CreateEventCommandHandleMock : IRequestHandler<CreateEventCommandMock, Result>
    {

        private readonly IValidator<CreateEventCommandMock> _validator;
        private readonly IEventRepository _eventRepository;
        private ILogger<CreateEventCommandHandleMock> _logger;


        public CreateEventCommandHandleMock(IEventRepository eventRepository,
                                           IValidator<CreateEventCommandMock> validator,
                                           ILogger<CreateEventCommandHandleMock> logger)
        {
            _eventRepository = eventRepository;
            _validator = validator;
            _logger = logger;
        }



        public async Task<Result> Handle(CreateEventCommandMock request, CancellationToken cancellationToken)
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
                MaxParticipants = request.MaxParticipants,
                OrganizerId = request.OrganizerId

            };

            try
            {
                await _eventRepository.AddAsync(new_event, cancellationToken);
                await _eventRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Мероприятие {Title} успешно создано", request.Title);
                return Result.Success();
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "Ошибка сохранения: {Inner}", inner);
                return Result.Failure($"Ошибка сохранения: {inner}", 500);
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