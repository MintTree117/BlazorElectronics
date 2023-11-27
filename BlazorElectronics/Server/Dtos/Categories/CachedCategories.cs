using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Dtos.Categories;

public sealed class CachedCategories : LocallyCachedObject
{
    public IReadOnlySet<int> PrimarySet { get; init; } = new HashSet<int>();
    public IReadOnlySet<int> SecondarySet { get; init; } = new HashSet<int>();
    public IReadOnlySet<int> TertiarySet { get; init; } = new HashSet<int>();
    
    public IReadOnlyList<CategoryResponse> PrimaryResponses { get; init; } = new List<CategoryResponse>();
    public IReadOnlyList<CategoryResponse> SecondaryResponses { get; init; } = new List<CategoryResponse>();
    public IReadOnlyList<CategoryResponse> TertiaryResponses { get; init; } = new List<CategoryResponse>();
}