using exam_system.Domain.Entities.Quizzes;

namespace exam_system.Features.Diplomas.GetDiplomaDetail.DTOs
{
    public class GetDiplomaDto
    {
        public Guid DiplomaId { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<QuizDto> Quizzes { get; set; }
    }
}
