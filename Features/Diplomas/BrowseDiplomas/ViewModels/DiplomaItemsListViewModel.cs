namespace exam_system.Features.Diplomas.BrowseDiplomas.ViewModels
{
    public class DiplomaItemsListViewModel
    {
        public Guid DiplomaID { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int TotalQuizzes { get; set; }
        public int CompletedQuizzes { get; set; }
    }
}
