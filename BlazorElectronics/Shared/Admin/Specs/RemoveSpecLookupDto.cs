namespace BlazorElectronics.Shared.Admin.Specs;

public sealed class RemoveSpecLookupDto
{
    public RemoveSpecLookupDto()
    {
        
    }

    public RemoveSpecLookupDto( SpecLookupType type, int id )
    {
        SpecType = type;
        SpecId = id;
    }
    
    public SpecLookupType SpecType { get; set; }
    public int SpecId { get; set; }
}