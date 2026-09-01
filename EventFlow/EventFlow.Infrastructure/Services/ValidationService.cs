using EventFlow.Application.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Infrastructure.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ValidateAsync<T>(T model)
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();

            if (validator != null)
            {
                var validationResult = await validator.ValidateAsync(model);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    throw new ValidationException(string.Join("; ", errors));
                }
            }
        }
    }
}