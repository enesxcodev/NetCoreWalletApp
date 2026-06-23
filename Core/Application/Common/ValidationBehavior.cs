using FluentValidation;
using MediatR;
using Application.Common;

namespace Application.Common;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next();
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Hata raporlarından boş olmayanları ayıkla ve sadece hata mesajlarını  listeye doldur
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .Select(f => f.ErrorMessage)
            .ToList();

        if (failures.Count != 0)
        {
            if (typeof(TResponse).IsGenericType)
            {
                var genericType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result<>)
                    .MakeGenericType(genericType)
                    .GetMethods()
                    .First(m => m.Name == "Failure" && m.GetParameters()[0].ParameterType == typeof(List<string>));

                return (TResponse)failureMethod.Invoke(null, [failures])!;
            }
            return (TResponse)(object)Result.Failure(failures);
        }
        return await next();
    }
}