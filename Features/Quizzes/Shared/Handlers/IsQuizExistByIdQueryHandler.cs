using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.Shared.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.Shared.Handlers
{
    public class IsQuizExistByIdQueryHandler :
        IRequestHandler<IsQuizExistByIdQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<Quiz> _quizRepo;

        public IsQuizExistByIdQueryHandler(IGenericRepository<Quiz> quizRepo)
        {
            _quizRepo = quizRepo;
        }
        public async Task<RequestResponse<bool>> Handle(IsQuizExistByIdQuery request, CancellationToken cancellationToken)
        {
            var isQuizExist =await _quizRepo.GetAll()
                             .AnyAsync(quiz => quiz.Id == request.Id,cancellationToken);
            return RequestResponse<bool>.Ok(isQuizExist);
        }
    }
}
