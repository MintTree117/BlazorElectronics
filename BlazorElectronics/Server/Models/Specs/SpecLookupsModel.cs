namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupsModel
{
    public IEnumerable<short>? IntGlobalIds { get; init; }
    public IEnumerable<short>? StringGlobalIds { get; init; }
    public IEnumerable<short>? BoolGlobalIds { get; init; }
    public IEnumerable<short>? MultiGlobalIds { get; init; }
    
    public IEnumerable<SpecLookupCategoryModel>? IntCategories { get; init; }
    public IEnumerable<SpecLookupCategoryModel>? StringCategories { get; init; }
    public IEnumerable<SpecLookupCategoryModel>? BoolCategories { get; init; }
    public IEnumerable<SpecLookupCategoryModel>? MultiCategories { get; init; }

    public IEnumerable<SpecLookupNameModel>? IntNames { get; init; }
    public IEnumerable<SpecLookupNameModel>? StringNames { get; init; }
    public IEnumerable<SpecLookupNameModel>? BoolNames { get; init; }
    public IEnumerable<SpecLookupNameModel>? MultiNames { get; init; }
    
    public IEnumerable<SpecLookupIntFilterModel>? IntFilters { get; init; }
    public IEnumerable<SpecLookupStringValueModel>? StringValues { get; init; }
    public IEnumerable<SpecLookupStringValueModel>? MultiValues { get; init; }
}