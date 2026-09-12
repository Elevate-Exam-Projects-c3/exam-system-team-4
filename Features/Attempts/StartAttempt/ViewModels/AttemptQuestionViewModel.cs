using exam_system.Features.Attempts.StartAttempt.DTOs;

namespace exam_system.Features.Attempts.StartAttempt.ViewModels
{
    public class AttemptQuestionViewModel
    {
        public Guid Id { get; init; }

        public string Text { get; init; } = string.Empty;
        public List<AttemptOptionViewModel> Options { get; set; } = [];
    }
}
