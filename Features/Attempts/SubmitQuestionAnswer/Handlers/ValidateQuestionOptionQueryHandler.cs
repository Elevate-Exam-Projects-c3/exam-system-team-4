using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Attempts.SubmitQuestionAnswer.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Handlers
{
    public class ValidateQuestionOptionQueryHandler(IGenericRepository<QuestionOption> repository): IRequestHandler<ValidateQuestionOptionQuery, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(ValidateQuestionOptionQuery request, CancellationToken cancellationToken)
        {
            var optionExists = await repository
             .GetAll()
             .AnyAsync(
                 x => x.Id == request.SelectedOptionId &&
                      x.QuestionId == request.QuestionId,
                 cancellationToken);

            if (!optionExists)
            {
                return RequestResponse<bool>.Fail(
                    "Selected option does not belong to the question.",
                    400);
            }

            return RequestResponse<bool>.Ok(true);
        }
    }
}
