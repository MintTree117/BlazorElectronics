using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Dtos.Categories;

public sealed class CachedCategories : LocallyCachedObject
{
    public IReadOnlySet<int> PrimarySet { get; init; } = new HashSet<int>();
    public IReadOnlySet<int> SecondarySet { get; init; } = new HashSet<int>();
    public IReadOnlySet<int> TertiarySet { get; init; } = new HashSet<int>();

    public IReadOnlyList<PrimaryCategoryResponse> PrimaryResponses { get; init; } = new List<PrimaryCategoryResponse>();
    public IReadOnlyList<SecondaryCategoryResponse> SecondaryResponses { get; init; } = new List<SecondaryCategoryResponse>();
    public IReadOnlyList<TertiaryCategoryResponse> TertiaryResponses { get; init; } = new List<TertiaryCategoryResponse>();
}