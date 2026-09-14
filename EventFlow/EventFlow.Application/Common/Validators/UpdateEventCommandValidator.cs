using EventFlow.API.Controllers;
using FluentValidation;

namespace EventFlow.Application.Commands.EventCommands
{
    public class UpdateEventCommandMockValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandMockValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("Id мероприятия обязателен");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название обязательно")
                .MaximumLength(200).WithMessage("Название не должно превышать 200 символов");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Описание не должно превышать 2000 символов");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Некорректный тип мероприятия");

            RuleFor(x => x.Start)
                .NotEmpty().WithMessage("Дата начала обязательна");

            RuleFor(x => x.End)
                .NotEmpty().WithMessage("Дата окончания обязательна")
                .GreaterThan(x => x.Start).WithMessage("Дата окончания должна быть позже даты начала");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Город обязателен");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адрес обязателен");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Цена не может быть отрицательной");

            RuleFor(x => x.MaxParticipants)
                .GreaterThan(0).WithMessage("Количество участников должно быть больше 0");

            RuleFor(x => x.OrganizerId)
                .NotEmpty().WithMessage("Id организатора обязателен");
        }
    }
}