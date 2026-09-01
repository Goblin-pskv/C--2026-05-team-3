using EventFlow.Application.Queries.GetProfileQuery;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Common.Validators
{
    public class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
    {
        public GetProfileQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
        }
    }
}
