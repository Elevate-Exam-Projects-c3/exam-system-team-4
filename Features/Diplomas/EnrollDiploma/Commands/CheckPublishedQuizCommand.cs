using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Commands
{
    public class CheckPublishedQuizCommand : IRequest<RequestResponse<bool>>
    {
        public CheckPublishedQuizCommand(Guid diplomaId)
        {
            DiplomaId = diplomaId;
        }

        public Guid DiplomaId { get; set; }
    }
}