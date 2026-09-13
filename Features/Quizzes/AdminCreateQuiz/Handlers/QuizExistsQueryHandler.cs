using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminCreateQuiz.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Handlers
{
    public class QuizExistsQueryHandler : IRequestHandler<QuizExistsQuery, bool>
    {
        private readonly IGenericRepository<Quiz> _quizRepository;

        public QuizExistsQueryHandler(IGenericRepository<Quiz> quizRepository)
        {
            _quizRepository = quizRepository;
        }

        public async Task<bool> Handle(QuizExistsQuery request, CancellationToken cancellationToken)
        {
           var quiz =await  _quizRepository.GetAll()
                .Where(q => q.Id == request.QuizId)
                .FirstOrDefaultAsync();

            return quiz != null;
        }
    }
}
