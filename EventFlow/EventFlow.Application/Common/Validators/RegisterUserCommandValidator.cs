using EventFlow.Application.Commands.RegisterCommand;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Common.Validators
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Некорректный формат Email");
            RuleFor(x => x.PasswordHash).NotEmpty().MinimumLength(6);
        }
    }
}