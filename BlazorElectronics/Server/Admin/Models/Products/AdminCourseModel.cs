namespace BlazorElectronics.Server.Admin.Models.Products;

public sealed class AdminCourseModel : AdminProductModel
{
    public string Instructors { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public int DurationWeeks { get; set; }
    public bool HasSubtitles { get; set; }
    public List<int> VideoAccessibility { get; set; } = new();
    public List<int> Certifications { get; set; } = new();
}