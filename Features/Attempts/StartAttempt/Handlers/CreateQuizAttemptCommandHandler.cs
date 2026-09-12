using exam_system.Domain.Entities.Attempts;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Attempts.Extentions;
using exam_system.Features.Attempts.StartAttempt.Commands;
using exam_system.Features.Attempts.StartAttempt.DTOs;
using exam_system.Features.Diplomas.SharedRequests.Queries;
using exam_system.Features.Quizzes.Shared.Queries;
using exam_system.Features.Shared;
using exam_system.Features.Students.StudentExistence.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Handlers
{
    public class CreateQuizAttemptCommandHandler : IRequestHandler<CreateQuizAttemptCommand, RequestResponse>
    {
        private readonly IMediator _mediator;
        private readonly IGenericRepository<QuizAttempt> _quizAttemptRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateQuizAttemptCommandHandler(IMediator mediator,
                                               IGenericRepository<QuizAttempt> QuizAttemptRepo,
                                               IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _quizAttemptRepo = QuizAttemptRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse> Handle(CreateQuizAttemptCommand request, CancellationToken cancellationToken)
        {
            //validate QuizId
            var isQuizExist = await _mediator.Send(new IsQuizExistByIdQuery(request.QuizId));
            if (isQuizExist is null || !isQuizExist.Success)
            {
                return RequestResponse.Fail(
                    "Quiz not found.",
                    404,
                    new Dictionary<string, string[]>
                    {
                        ["StartDate"] = ["Quiz has not started yet."]
                    });
            }
            //validate studentId
            var isStudentExist =await _mediator.Send(new IsStudentExistQuery(request.StudentId));
            if (isStudentExist is null ||! isStudentExist.Data)
            {
                return RequestResponse.Fail(
                    "Student not found.",
                    404,
                    new Dictionary<string, string[]>
                    {
                        ["StudentId"] = ["The specified student was not found."]
                    });           
            }
            _quizAttemptRepo.Add(new QuizAttempt
            {
                StudentId = request.StudentId,
                QuizId = request.QuizId,
                StartTime = request.StartTime,
                Status = request.attemptStatus,
                Deadline = request.Deadline
            });

           var isCreated= (await _unitOfWork.SaveChangesAsync())>0;
             if (isCreated )
            {
                return RequestResponse.Ok();
            }
             else
            {
                return RequestResponse.Fail(
                     "Failed to create quiz attempt.",
                     500,
                     new Dictionary<string, string[]>
                     {
                         ["QuizAttempt"] = ["The quiz attempt could not be created."]
                     });
            }
        }
    }
}