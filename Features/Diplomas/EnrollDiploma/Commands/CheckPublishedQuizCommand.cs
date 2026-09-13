using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Commands
{
    public class CheckPublishedQuizCommand : IRequest<bool>
    {
        public Guid DiplomaId { get; set; }
    }
}