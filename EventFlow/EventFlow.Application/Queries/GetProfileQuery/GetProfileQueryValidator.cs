using EventFlow.Application.Queries.GetProfileQuery;
using FluentValidation;

namespace EventFlow.Application.Queries.GetProfileQuery
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