namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupsModel
{
    public IEnumerable<short>? IntGlobalIds { get; set; }
    public IEnumerable<short>? StringGlobalIds { get; set; }
    public IEnumerable<short>? BoolGlobalIds { get; set; }
    
    public IEnumerable<SpecLookupSingleCategoryModel>? IntCategories { get; set; }
    public IEnumerable<SpecLookupSingleCategoryModel>? StringCategories { get; set; }
    public IEnumerable<SpecLookupSingleCategoryModel>? BoolCategories { get; set; }

    public IEnumerable<SpecLookupNameModel>? IntNames { get; set; }
    public IEnumerable<SpecLookupNameModel>? StringNames { get; set; }
    public IEnumerable<SpecLookupNameModel>? BoolNames { get; set; }
    
    public IEnumerable<SpecLookupFilterModel>? IntFilters { get; set; }
    public IEnumerable<SpecLookupFilterModel>? StringFilters { get; set; }

    public IEnumerable<short>? MultiTablesGlobal { get; set; }
    public IEnumerable<SpecLookupMultiTableCategoryModel>? MultiTableCategories { get; set; }
    public IEnumerable<SpecLookupMultiTableModel>? MultiTables { get; set; }
}