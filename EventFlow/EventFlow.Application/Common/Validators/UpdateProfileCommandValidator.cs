using EventFlow.Application.Commands.UpdateProfileCommand;
using FluentValidation;

namespace EventFlow.Application.Common.Validators
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required")
                .NotEqual(Guid.Empty).WithMessage("UserId cannot be empty");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters")
                .Matches(@"^[a-zA-Zа-яА-Я\s\-]+$").WithMessage("First name can only contain letters, spaces and hyphens");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters")
                .Matches(@"^[a-zA-Zа-яА-Я\s\-]+$").WithMessage("Last name can only contain letters, spaces and hyphens");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format. Use format: +1234567890")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }
    }
}
