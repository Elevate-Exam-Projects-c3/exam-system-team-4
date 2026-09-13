using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Diplomas.EnrollDiploma.Interfaces;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class CheckPublishedQuizCommandHandler
        : IRequestHandler<CheckPublishedQuizCommand, bool>
    {
        private readonly IDiploma _diplomaRepository;

        public CheckPublishedQuizCommandHandler(
            IDiploma diplomaRepository)
        {
            _diplomaRepository = diplomaRepository;
        }

        public async Task<bool> Handle(
            CheckPublishedQuizCommand request,
            CancellationToken cancellationToken)
        {
            return await _diplomaRepository.HasPublishedQuizAsync(
                request.DiplomaId,
                cancellationToken);
        }
    }
}