namespace exam_system.Features.Attempts.StartAttempt.DTOs
{
    public class StartAttemptResponseDto
    {
        public Guid AttemptId { get; init; }
        public Guid QuizId { get; init; }

        public DateTimeOffset StartTime { get; init; }

        public DateTimeOffset Deadline { get; init; }

        public int DurationMinutes { get; init; }

        public List<AttemptQuestionDto> Questions { get; set; } = [];
    }
}

