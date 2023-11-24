namespace BlazorElectronics.Shared.Admin.Specs;

public sealed class SpecLookupGetEditDto
{
    public SpecLookupGetEditDto()
    {
        
    }
    public SpecLookupGetEditDto( SpecLookupType type, int id )
    {
        SpecType = type;
        SpedId = id;
    }
    
    public SpecLookupType SpecType { get; set; }
    public int SpedId { get; set; }
}