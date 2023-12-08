namespace BlazorElectronics.Shared.SpecLookups;

public sealed class SpecEdit : ICrudEdit
{
    public int SpecId { get; set; }
    public string SpecName { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public bool IsAvoid { get; set; }
    public List<int> PrimaryCategories { get; set; } = new();
    public string ValuesByIdAsString { get; set; } = string.Empty;
    
    public void SetId( int id )
    {
        SpecId = id;
    }
}