namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupTableMetaModel
{
    public IEnumerable<int>? ExplicitIntGlobalIds { get; set; }
    public IEnumerable<int>? ExplicitStringGlobalIds { get; set; }
    public IEnumerable<ExplicitProductSpecCategory>? ExplicitIntCategories { get; set; }
    public IEnumerable<ExplicitProductSpecCategory>? ExplicitStringCategories { get; set; }
    public IEnumerable<ExplicitProductSpecName>? ExplicitIntNames { get; set; }
    public IEnumerable<ExplicitProductSpecName>? ExplicitStringNames { get; set; }
    
    public IEnumerable<int>? DyanmicGlobalIds { get; set; }
    public IEnumerable<DynamicSpecTableCategory>? DynamicCategories { get; set; }
    public IEnumerable<DynamicSpecTableMeta>? DynamicTableMeta { get; set; }
    //public IEnumerable<IEnumerable<DynamicSpecValue>?>? DyanmicValues { get; set; }
}