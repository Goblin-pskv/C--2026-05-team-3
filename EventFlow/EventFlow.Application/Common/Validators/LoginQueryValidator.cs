using EventFlow.Application.Queries.LoginQuery;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Common.Validators
{
    public class LoginQueryValidator : AbstractValidator<LoginQuery>
    {
        public LoginQueryValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Некорректный формат Email");
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}