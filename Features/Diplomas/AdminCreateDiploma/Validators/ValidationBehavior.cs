//using FluentValidation;
//using MediatR;

//namespace exam_system.Features.Diplomas.AdminCreateDiploma.Validators
//{
//    public class ValidationBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
//    {
//        private readonly IEnumerable<IValidator<TRequest>> _validators;
//        private readonly ILogger _logger;

//        public ValidationBehavior(
//            IEnumerable<IValidator<TRequest>> validators, ILogger logger)
//        {
//            _validators = validators;
//            _logger = logger;
//        }

//        public async Task<TResponse> Handle(
//            TRequest request,
//            RequestHandlerDelegate<TResponse> next,
//            CancellationToken cancellationToken)
//        {
//            if (_validators.Any())
//            {
//                var context = new ValidationContext<TRequest>(request);

//                var results = await Task.WhenAll(
//                    _validators.Select(
//                        validator => validator.ValidateAsync(
//                            context,
//                            cancellationToken)));

//                var failures = results
//                    .SelectMany(x => x.Errors)
//                    .Where(x => x != null)
//                    .ToList();

//                if (failures.Any())
//                {
//                    throw new ValidationException(failures);
//                }
//            }

//            return await next();
//        }

//        public async Task<TResponse> Handle(
//    TRequest request,
//    CancellationToken cancellationToken,
//    RequestHandlerDelegate<TResponse> next)
//        {
//            if (_validators.Any())
//            {
//                var context = new ValidationContext<TRequest>(request);

//                var validationResults = await Task.WhenAll(
//                    _validators.Select(
//                        validator => validator.ValidateAsync(
//                            context,
//                            cancellationToken)));

//                var errors = validationResults
//                    .SelectMany(result => result.Errors)
//                    .Where(error => error != null)
//                    .ToList();

//                if (errors.Any())
//                {
//                    _logger.LogError("Title must be between 3 - 100 characters");
//                }
//            }

//            return await next();
//        }
//    }
//}
