using exam_system.Common.Enums;

namespace exam_system.Features.Quizzes.Shared.DTOs
{
    public class QuizStartAttemptDto
    {
        public Guid DiplomaId { get; init; }
        public Guid QuizId { get; init; }
        public int? MaxAttempts { get; init; }
        public int DurationMinutes { get; init; }
        public QuizStatus Status { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }
}
