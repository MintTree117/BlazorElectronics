namespace BlazorElectronics.Server.Models.Products.Seed;

public sealed class CourseSeedModel : ProductSeedModel
{
    public string Instructors { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public int DurationWeeks { get; set; }
    public bool HasSubtitles { get; set; }
    public List<int> VideoAccessibility { get; set; } = new();
    public List<int> Certifications { get; set; } = new();
}