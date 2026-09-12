using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Attempts.StartAttempt.DTOs;
using exam_system.Features.Questions.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Questions.Handlers
{
    public class GetQuizQuestionsAndOptionsForStartAttemptQueryHandler
    : IRequestHandler<
        GetQuizQuestionsAndOptionsForStartAttemptQuery,
        RequestResponse<List<AttemptQuestionDto>>>
    {
        private readonly IGenericRepository<Question> _questionRepo;

        public GetQuizQuestionsAndOptionsForStartAttemptQueryHandler(
            IGenericRepository<Question> questionRepo)
        {
            _questionRepo = questionRepo;
        }

        public async Task<RequestResponse<List<AttemptQuestionDto>>> Handle(
            GetQuizQuestionsAndOptionsForStartAttemptQuery request,
            CancellationToken cancellationToken)
        {
            var questions = await _questionRepo.GetAll()
                .Where(q => q.QuizId == request.QuizId)
                .OrderBy(q => Guid.NewGuid())
                .Select(q => new AttemptQuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,

                    Options = q.Options
                        .Select(o => new AttemptOptionDto
                        {
                            Id = o.Id,
                            Text = o.OptionText
                        }).OrderBy(q => Guid.NewGuid())
                        .ToList()
                })
                .ToListAsync(cancellationToken);
            return RequestResponse<List<AttemptQuestionDto>>.Ok(questions);
        }
    }
}