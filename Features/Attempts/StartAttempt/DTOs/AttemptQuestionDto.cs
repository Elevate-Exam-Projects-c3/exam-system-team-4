namespace exam_system.Features.Attempts.StartAttempt.DTOs
{
    public class AttemptQuestionDto
    {
        public Guid Id { get; init; }

        public string Text { get; init; } = string.Empty;
        public List<AttemptOptionDto> Options { get; set; } = [];


    }
}