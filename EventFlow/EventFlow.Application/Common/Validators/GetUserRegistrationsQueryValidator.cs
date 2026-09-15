using EventFlow.Application.Queries.RegistrationQueries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Common.Validators
{
    public class GetUserRegistrationsQueryMockValidator
        : AbstractValidator<GetUserRegistrationsQuery>
    {
        public GetUserRegistrationsQueryMockValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId обязателен");
        }
    }
}
