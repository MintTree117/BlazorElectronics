using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Server.Dtos.Categories;

public sealed class CategoriesDto : LocallyCachedObject
{
    public CategoriesDto( Dictionary<short,short> pIds, Dictionary<short, short> sIds, Dictionary<short, short> tIds, List<PrimaryCategoryResponse> pResponses, List<SecondaryCategoryResponse> sResponses, List<TertiaryCategoryResponse> tResponses )
    {
        PrimaryIds = pIds;
        SecondaryIds = sIds;
        TertiaryIds = tIds;

        PrimaryResponses = pResponses;
        SecondaryResponses = sResponses;
        TertiaryResponses = tResponses;
    }

    public IReadOnlyDictionary<short, short> PrimaryIds { get; init; }
    public IReadOnlyDictionary<short, short> SecondaryIds { get; init; }
    public IReadOnlyDictionary<short, short> TertiaryIds { get; init; }
    
    public IReadOnlyList<PrimaryCategoryResponse> PrimaryResponses { get; set; }
    public IReadOnlyList<SecondaryCategoryResponse> SecondaryResponses { get; set; }
    public IReadOnlyList<TertiaryCategoryResponse> TertiaryResponses { get; set; }
}