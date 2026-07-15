using EventFlow.Application.Commands.UpdateProfileCommand;
using FluentValidation;

namespace EventFlow.Application.Common.Validators
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Некорректный формат Email"); ;
            RuleFor(x => x.PhoneNumber).MaximumLength(20);
        }
    }
}