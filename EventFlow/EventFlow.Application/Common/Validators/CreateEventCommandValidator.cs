using EventFlow.Application.Commands.EventCommands;
using FluentValidation;
using System;

namespace EventFlow.Application.Common.Validators
{
    public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid event type");

            RuleFor(x => x.Start)
                .NotEmpty().WithMessage("Start date is required")
                .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in future");

            RuleFor(x => x.End)
                .NotEmpty().WithMessage("End date is required")
                .GreaterThan(x => x.Start).WithMessage("End date must be after start date");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative");

            RuleFor(x => x.MaxParticipants)
                .GreaterThan(0).WithMessage("Max participants must be greater than 0");

            RuleFor(x => x.OrganizerName)
                .NotEmpty().WithMessage("Organizer name is required")
                .MaximumLength(100).WithMessage("Organizer name must not exceed 100 characters");

            RuleFor(x => x.IsPublished)
                .NotNull().WithMessage("IsPublished must be specified");
        }
    }
}