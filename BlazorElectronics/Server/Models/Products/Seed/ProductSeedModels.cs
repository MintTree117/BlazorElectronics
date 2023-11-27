namespace BlazorElectronics.Server.Models.Products.Seed;

public class ProductSeedModels
{
    public List<BookSeedModel> Books { get; set; } = new();
    public List<SoftwareSeedSeedModel> Software { get; set; } = new();
    public List<GamesSeedModel> Games { get; set; } = new();
    public List<MoviesTvSeedModel> MoviesTv { get; set; } = new();
    public List<CourseSeedModel> Courses { get; set; } = new();
}