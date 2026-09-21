using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Queries
{
    public record ValidateQuestionOptionQuery(
    Guid QuestionId,
    Guid SelectedOptionId
) : IRequest<RequestResponse<bool>>;
}
