namespace exam_system.Features.Quizzes.Readiness;

public class ReadinessCheckDto
{

    public string Name { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string Message { get; set; } = string.Empty;
}