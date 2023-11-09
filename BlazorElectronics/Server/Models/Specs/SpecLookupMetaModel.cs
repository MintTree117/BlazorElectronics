namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupMetaModel
{
    public IEnumerable<short>? IntGlobalIds { get; set; }
    public IEnumerable<short>? StringGlobalIds { get; set; }
    public IEnumerable<short>? BoolGlobalIds { get; set; }
    
    public IEnumerable<RawSpecCategoryModel>? IntCategories { get; set; }
    public IEnumerable<RawSpecCategoryModel>? StringCategories { get; set; }
    public IEnumerable<RawSpecCategoryModel>? BoolCategories { get; set; }

    public IEnumerable<RawSpecNameModel>? IntNames { get; set; }
    public IEnumerable<RawSpecNameModel>? StringNames { get; set; }
    public IEnumerable<RawSpecNameModel>? BoolNames { get; set; }
    
    public IEnumerable<IntFilterModel>? IntFilters { get; set; }
    public IEnumerable<StringSpecValueModel>? StringValues { get; set; }

    public IEnumerable<short>? DyanmicGlobalTableIds { get; set; }
    public IEnumerable<DynamicSpecTableCategoryModel>? DynamicTableCategories { get; set; }
    public IEnumerable<DynamicSpecTableMetaModel>? DynamicTables { get; set; }
}