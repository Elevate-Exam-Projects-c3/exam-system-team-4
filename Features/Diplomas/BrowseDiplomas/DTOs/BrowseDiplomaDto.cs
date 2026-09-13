namespace exam_system.Features.Diplomas.BrowseDiplomas.DTOs
{
    public class BrowseDiplomaDto
    {

            public Guid Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int CompletedQuizzes { get; set; }
            public int TotalQuizzes { get; set; }

        
    }
}
