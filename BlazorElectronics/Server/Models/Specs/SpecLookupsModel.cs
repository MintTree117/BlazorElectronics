namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupsModel
{
    public IEnumerable<short>? IntGlobalIds { get; init; }
    public IEnumerable<short>? StringGlobalIds { get; init; }
    public IEnumerable<short>? BoolGlobalIds { get; init; }
    
    public IEnumerable<SpecLookupSingleCategoryModel>? IntCategories { get; init; }
    public IEnumerable<SpecLookupSingleCategoryModel>? StringCategories { get; init; }
    public IEnumerable<SpecLookupSingleCategoryModel>? BoolCategories { get; init; }

    public IEnumerable<SpecLookupNameModel>? IntNames { get; init; }
    public IEnumerable<SpecLookupNameModel>? StringNames { get; init; }
    public IEnumerable<SpecLookupNameModel>? BoolNames { get; init; }
    
    public IEnumerable<SpecLookupIntFilterModel>? IntFilters { get; init; }
    public IEnumerable<SpecLookupStringValueModel>? StringValues { get; init; }

    public IEnumerable<short>? MultiTablesGlobal { get; init; }
    public IEnumerable<SpecLookupMultiTableCategoryModel>? MultiTableCategories { get; init; }
    public IEnumerable<SpecLookupMultiTableModel>? MultiTables { get; init; }
}