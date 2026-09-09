namespace exam_system.Features.Quizzes.AdminCreateQuiz.ViewModels
{
    public record CreateQuizViewModel(Guid DiplomaId,
                                  string Title,
                                  int DurationMinutes,
                                  DateTime StartDate,
                                  DateTime EndDate,
                                  string? Instructions,
                                  int PassScore = 60,
                                  int? MaxAttempts = null
                                  );
}
