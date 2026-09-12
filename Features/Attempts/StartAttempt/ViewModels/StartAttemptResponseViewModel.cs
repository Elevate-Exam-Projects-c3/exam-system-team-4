using exam_system.Features.Attempts.StartAttempt.DTOs;

namespace exam_system.Features.Attempts.StartAttempt.ViewModels
{
    public class StartAttemptResponseViewModel
    {
        public Guid AttemptId { get; init; }
        public Guid QuizId { get; init; }

        public DateTimeOffset StartTime { get; init; }

        public DateTimeOffset Deadline { get; init; }

        public int DurationMinutes { get; init; }

        public List<AttemptQuestionViewModel> Questions { get; set; } = [];
    }
}
