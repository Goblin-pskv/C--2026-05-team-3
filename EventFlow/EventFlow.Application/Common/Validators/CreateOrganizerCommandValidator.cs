using EventFlow.Application.Commands.OrganizerCommands;
using FluentValidation;

namespace EventFlow.Application.Common.Validators
{
    public class CreateOrganizerCommandValidator : AbstractValidator<CreateOrganizerCommand>
    {
        public CreateOrganizerCommandValidator()
        {
            RuleFor(x => x.CompanyName)
                .MaximumLength(200).WithMessage("Название компании не должно превышать 200 символов");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов");

            RuleFor(x => x.WebSite)
                .MaximumLength(200).WithMessage("Веб-сайт не должен превышать 200 символов");
                //.Matches(@"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$").WithMessage("Некорректный формат веб-сайта")
                //.When(x => !string.IsNullOrEmpty(x.WebSite));
        }
    }
}