using EventFlow.Application.Common;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

                // Проверяем, является ли TResponse типом Result (без generic)
                if (typeof(TResponse) == typeof(Result))
                {
                    return (TResponse)(object)Result.Failure(errorMessage);
                }
                // Проверяем, является ли TResponse типом Result<T>
                else if (typeof(TResponse).IsGenericType &&
                         typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var genericType = typeof(TResponse).GetGenericArguments()[0];
                    var failureMethod = typeof(Result<>).MakeGenericType(genericType)
                        .GetMethod("Failure", new[] { typeof(string) });

                    if (failureMethod != null)
                        return (TResponse)failureMethod.Invoke(null, new object[] { errorMessage });
                }

                // Если это не Result, выбрасываем исключение
                throw new ValidationException(errorMessage);
            }

            return await next();
        }
    }
}