namespace BlazorElectronics.Shared.Admin.Specs;

public sealed class GetSpecLookupEditDto
{
    public GetSpecLookupEditDto()
    {
        
    }
    public GetSpecLookupEditDto( SpecLookupType type, int id )
    {
        SpecType = type;
        SpedId = id;
    }
    
    public SpecLookupType SpecType { get; set; }
    public int SpedId { get; set; }
}