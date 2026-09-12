namespace exam_system.Features.Quizzes.Readiness;

public class QuizReadinessDto
{

    public Guid QuizId { get; set; }


    public bool CanPublish { get; set; }


    public List<ReadinessCheckDto> Checks { get; set; } = [];
}