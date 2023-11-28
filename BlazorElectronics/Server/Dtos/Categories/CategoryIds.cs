namespace BlazorElectronics.Server.Dtos.Categories;

public sealed class CategoryIds
{
    public IReadOnlySet<int> PrimarySet { get; init; } = new HashSet<int>();
    public IReadOnlySet<int> SecondarySet { get; init; } = new HashSet<int>();
    public IReadOnlySet<int> TertiarySet { get; init; } = new HashSet<int>();
}