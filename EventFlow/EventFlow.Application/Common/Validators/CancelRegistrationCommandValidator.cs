using EventFlow.Application.Commands.RegistrationCommands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Common.Validators
{
    public class CancelRegistrationCommandMockValidator
       : AbstractValidator<CancelRegistrationCommand>
    {
        public CancelRegistrationCommandMockValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId обязателен");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId обязателен");
        }
    }
}
