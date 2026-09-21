using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.SubmitAttempt.Commands;
using exam_system.Features.Attempts.SubmitAttempt.DTOs;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Attempts.SubmitAttempt.Handlers
{
    public class SubmitQuizAttemptCommandHandler
    : IRequestHandler<SubmitQuizAttemptCommand, RequestResponse>
    {
        private readonly IGenericRepository<QuizAttempt> _attemptRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubmitQuizAttemptCommandHandler(
            IGenericRepository<QuizAttempt> attemptRepository,
            IUnitOfWork unitOfWork)
        {
            _attemptRepository = attemptRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse> Handle(
            SubmitQuizAttemptCommand request,
            CancellationToken cancellationToken)
        {
            var attempt = await _attemptRepository.GetByIdAsync(request.AttemptId,cancellationToken);

            if (attempt is null)
            {
                return RequestResponse.Fail("Attempt not found.",404);
            }

            attempt.Status = request.AttemptStatus;
            attempt.Score = request.Score;
            attempt.Passed = request.Passed;
            attempt.SubmittedAt = request.SubmittedAt;

            _attemptRepository.Update(attempt);

           var resultAfterSave =await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (resultAfterSave > 0)
                return RequestResponse.Ok();
            else
                return RequestResponse.Fail("failed to submit");
        }
    }
}
