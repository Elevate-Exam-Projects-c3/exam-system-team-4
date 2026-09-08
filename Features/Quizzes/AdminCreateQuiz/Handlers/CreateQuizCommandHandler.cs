using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Diplomas.SharedRequests.Queries;
using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Handlers
{
    public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, RequestResponse>
    {
        private readonly IGenericRepository<Quiz> _quizRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CreateQuizCommandHandler(IGenericRepository<Quiz> quizRepo, IMediator mediator,IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _quizRepo = quizRepo;
        }

        public async Task<RequestResponse> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
        {
            //is deploma exist
            var isDiplomaExist = (await _mediator.Send(new CheckDiplomaExistenceById(request.DiplomaId), cancellationToken)).Data;
            if (!isDiplomaExist)
            {
                return RequestResponse.Fail($"Diploma With Id={request.DiplomaId} Is Not Found",
                    404,
                    new Dictionary<string, string[]>
                    {
                        ["DiplomaId"] = [$"Diploma With Id ={request.DiplomaId} Is Not Found "]
                    });
            }

            //create 
            _quizRepo.Add(new Quiz
            {
                DiplomaId = request.DiplomaId,
                Title = request.Title,
                DurationMinutes = request.DurationMinutes,
                Instructions = request.Instructions,
                PassScore = request.PassScore,
                MaxAttempts = request.MaxAttempts,
                Status = QuizStatus.Draft,
                CreatedAt = DateTime.UtcNow
            });
            var result=await _unitOfWork.SaveChangesAsync(cancellationToken);
            if(result>0)
            return RequestResponse.Ok();
            else
                return RequestResponse.Fail("Failed to create => 500 ",500);

            //
        }
    }
}
