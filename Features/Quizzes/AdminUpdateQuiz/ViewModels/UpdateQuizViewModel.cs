namespace exam_system.Features.Quizzes.AdminUpdateQuiz.ViewModels
{
    public record UpdateQuizViewModel(
                                  string Title,
                                  int DurationMinutes,
                                  DateTime StartDate,
                                  DateTime EndDate,
                                  string? Instructions,
                                  int PassScore,
                                  int? MaxAttempts);



}
