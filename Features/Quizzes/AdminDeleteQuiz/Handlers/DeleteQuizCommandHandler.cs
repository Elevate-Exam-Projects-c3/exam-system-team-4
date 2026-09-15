using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Attempts.SharedRequests.Queries;
using exam_system.Features.Quizzes.AdminDeleteQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminDeleteQuiz.Handlers
{
    public class DeleteQuizCommandHandler : IRequestHandler<DeleteQuizCommand, RequestResponse>
    {
        private readonly IGenericRepository<Quiz> _quizRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteQuizCommandHandler(IGenericRepository<Quiz> quizRepo,
                                        IMediator mediator,
                                        IUnitOfWork unitOfWork)
        {
            _quizRepo = quizRepo;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
        {
           var quiz=await _quizRepo.GetAll()
                     .Where(quiz=>quiz.Id==request.Id)
                     .FirstOrDefaultAsync(cancellationToken);
            if (quiz is null)
            {
                return RequestResponse.Fail(
                    "Quiz not found.",
                    404,
                    new Dictionary<string, string[]>
                    {
                        { "QuizId", ["The specified quiz was not found."] }
                    });
            }
            //Is it Deleted
            if(quiz.IsDeleted)
                return RequestResponse.Ok();

            //published
            if (quiz.Status == QuizStatus.Published)
            {
                return RequestResponse.Fail(
                    "Published quiz cannot be deleted.",
                    409,
                    new Dictionary<string, string[]>
                    {
                        { "QuizStatus", ["The quiz must be unpublished before it can be deleted."] }
                    });
            }
            //in progress attempts
            var inProgressAttemptsCountResponse = await _mediator.Send(new GetInProgressQuizAttemptsCountQuery(request.Id));
            if (inProgressAttemptsCountResponse is null ||
                !inProgressAttemptsCountResponse.Success)
            {
                return RequestResponse.Fail(
                    "Failed to check in-progress attempts.",
                    500);
            }

            if (inProgressAttemptsCountResponse.Data > 0)
            {
                return RequestResponse.Fail(
                    "Quiz cannot be deleted while there are in-progress attempts.",
                    409,
                    new Dictionary<string, string[]>
                    {
                        { "InProgressAttempts",
                        ["The quiz has one or more in-progress attempts and cannot be deleted."]}
                    });
            }
            //update
            _quizRepo.Delete(quiz);

            var isSaved = (await _unitOfWork.SaveChangesAsync(cancellationToken)) > 0;
            if(isSaved)
            {
                return RequestResponse.Ok();
            }
            else
            {
                return RequestResponse.Fail("Failed to delete the quiz.", 500);
            }
        }
    }
}
