namespace BlazorElectronics.Shared.Admin.Specs;

public sealed class SpecLookupRemoveDto
{
    public SpecLookupRemoveDto()
    {
        
    }

    public SpecLookupRemoveDto( SpecLookupType type, int id )
    {
        SpecType = type;
        SpecId = id;
    }
    
    public SpecLookupType SpecType { get; set; }
    public int SpecId { get; set; }
}