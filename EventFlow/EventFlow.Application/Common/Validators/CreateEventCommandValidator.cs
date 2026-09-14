using EventFlow.Application.Commands.EventCommands;
using EventFlow.Application.Commands.RegisterCommand;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Common.Validators
{
    public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название мероприятия обязательно")
                .MaximumLength(200).WithMessage("Название не должно превышать 200 символов")
                .MinimumLength(3).WithMessage("Название должно содержать минимум 3 символа");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание обязательно")
                .MaximumLength(2000).WithMessage("Описание не должно превышать 2000 символов");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Некорректный тип мероприятия");

            RuleFor(x => x.Start)
                .NotEmpty().WithMessage("Дата начала обязательна")
                .GreaterThan(DateTime.UtcNow).WithMessage("Дата начала должна быть в будущем");

            RuleFor(x => x.End)
                .NotEmpty().WithMessage("Дата окончания обязательна")
                .GreaterThan(x => x.Start).WithMessage("Дата окончания должна быть позже даты начала");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Город обязателен")
                .MaximumLength(100).WithMessage("Название города не должно превышать 100 символов");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адрес обязателен")
                .MaximumLength(300).WithMessage("Адрес не должен превышать 300 символов");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Цена не может быть отрицательной");

            RuleFor(x => x.MaxParticipants)
                .GreaterThan(0).WithMessage("Количество участников должно быть больше 0")
                .LessThanOrEqualTo(100000).WithMessage("Количество участников слишком велико");

            RuleFor(x => x)
                .Must(x => x.Start != x.End)
                .WithMessage("Дата начала и окончания не могут совпадать")
                .OverridePropertyName("Start");
        }

    }
}
