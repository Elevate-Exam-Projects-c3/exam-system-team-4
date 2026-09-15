using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace exam_system.Features.Shared.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request,CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        // لو مفيش Validator للطلب، كمّل للمرحلة التالية.
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = new List<ValidationFailure>();

        // شغّل كل Validators الخاصة بالطلب.
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(
                context,
                cancellationToken);

            failures.AddRange(result.Errors);
        }

        // لو فيه أخطاء أوقف الـPipeline.
        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        // البيانات صحيح انتقل إلى TransactionBehavior.
        return await next();
    }
}