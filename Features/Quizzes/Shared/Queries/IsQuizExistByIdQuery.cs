using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.Shared.Queries
{
    public record IsQuizExistByIdQuery(Guid Id) : IRequest<RequestResponse<bool>>;
    
}
