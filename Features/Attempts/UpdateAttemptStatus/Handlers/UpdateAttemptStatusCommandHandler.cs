using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.UpdateAttemptStatus.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.UpdateAttemptStatus.Handlers
{
    public class UpdateAttemptStatusCommandHandler
        : IRequestHandler<UpdateAttemptStatusCommand, RequestResponse>
    {
        private readonly IGenericRepository<QuizAttempt> _quizAttemptRepo;

        public UpdateAttemptStatusCommandHandler(IGenericRepository<QuizAttempt> quizAttemptRepo)
        {
            _quizAttemptRepo = quizAttemptRepo;
        }

        public async Task<RequestResponse> Handle(
            UpdateAttemptStatusCommand request,
            CancellationToken cancellationToken)
        {
            var attempt = await _quizAttemptRepo.GetAll()
                                                .Where(attempt => attempt.Id == request.Id &&
                                                       attempt.Status != request.newStatus)
                                                .ExecuteUpdateAsync(setters => setters
                                                        .SetProperty(attempt => attempt.Status, request.newStatus)
                                                        .SetProperty(attempt => attempt.UpdatedAt, DateTime.UtcNow),
                                                        cancellationToken);

            if (attempt > 0)
                return RequestResponse.Ok();
            else
                return RequestResponse.Fail("Failed to Update Attempt status");
        }
    }
}
