namespace exam_system.Features.Diplomas.GetDiplomaDetail.DTOs
{
    public class AttemptDto
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset Deadline { get; set; }

        public int DurationMinutes { get; set; }
        public int ShuffleSeed { get; init; }
        public Guid? LastAnsweredQuestionId { get; set; }
    }
}
