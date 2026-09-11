namespace exam_system.Features.Quizzes.Shared.Validators
{
    public interface IQuizScheduleRequest
    {
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        int DurationMinutes { get; }
    }
}
