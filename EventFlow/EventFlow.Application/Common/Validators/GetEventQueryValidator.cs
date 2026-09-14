using EventFlow.Application.Queries.EventQueries;
using FluentValidation;

namespace EventFlow.Application.Queries.EventValidators
{
    public class GetEventQueryMockValidator : AbstractValidator<GetEventQueryMock>
    {
        public GetEventQueryMockValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId обязателен")
                .NotEqual(Guid.Empty).WithMessage("EventId не может быть пустым GUID");
        }
    }
}