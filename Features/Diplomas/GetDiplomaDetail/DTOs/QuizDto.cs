using exam_system.Domain.Entities.Attempts;

namespace exam_system.Features.Diplomas.GetDiplomaDetail.DTOs
{
    public class QuizDto
    {
        public  Guid QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int PassScore { get; set; } = 60;

        public int? MaxAttempts { get; set; }
        public int AttemptsUsed { get; set; }
        public bool HasInProgressAttempt { get; set; }
        public bool IsResumable { get; set; }
        public bool CanAttempt { get; set; }







        public List<AttemptDto> Attempts { get; set; } = new();
    }
}
