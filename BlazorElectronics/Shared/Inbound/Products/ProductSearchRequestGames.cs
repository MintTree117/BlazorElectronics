namespace BlazorElectronics.Shared.Inbound.Products;

public sealed class ProductSearchRequestGames : ProductSearchRequest
{
    public int? GameDeveloperId { get; set; }
    public int? MinEsrbRating { get; set; }
    public int? MaxEsrbRating { get; set; }
    public bool? HasMultiplayer { get; set; }
    public bool? HasInGamePurchases { get; set; }
    public bool? HasControllerSupport { get; set; }
    
    public List<int>? IncludeGamePlatformIds { get; set; }
    public List<(int, int)>? MinSystemRequirements { get; set; }
    public List<(int, int)>? MaxSystemRequirements { get; set; }
    public List<int>? AvoidOsRequirementIds { get; set; }
    public List<int>? IncludeOsRequirementIds { get; set; }
    
    public Dictionary<int, List<int>>? GamesDynamicFiltersInclude { get; set; }
    public Dictionary<int, List<int>>? GamesDynamicFiltersExclude { get; set; }
}