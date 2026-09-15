using EventFlow.Application.Queries.RegistrationQueries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Common.Validators
{
   
        public class GetEventRegistrationsQueryMockValidator
        : AbstractValidator<GetEventRegistrationsQuery>
        {
            public GetEventRegistrationsQueryMockValidator()
            {
                RuleFor(x => x.EventId)
                    .NotEmpty().WithMessage("EventId обязателен");
            }
        }
    
}
