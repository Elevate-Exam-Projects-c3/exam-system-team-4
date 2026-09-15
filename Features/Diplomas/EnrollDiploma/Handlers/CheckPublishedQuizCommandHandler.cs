using exam_system.Common.Enums;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class CheckPublishedQuizCommandHandler
        : IRequestHandler<CheckPublishedQuizCommand, RequestResponse<bool>>
    {
        private readonly IGenericRepository<Diploma> _repository;

        public CheckPublishedQuizCommandHandler(IGenericRepository<Diploma> repository)
        {
            _repository = repository;
        }

        public async Task<RequestResponse<bool>> Handle(CheckPublishedQuizCommand request, CancellationToken cancellationToken)
        {
            var hasPublishedQuizzes = await _repository.GetAll()
                .Where(d => d.Id == request.DiplomaId)
                .SelectMany(c => c.Quizzes)        
                .AnyAsync(q => q.Status == QuizStatus.Published, cancellationToken);

            return RequestResponse<bool>.Ok(hasPublishedQuizzes);
        }
    }
}