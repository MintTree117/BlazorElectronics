namespace BlazorElectronics.Shared.Admin.SpecsSingle;

public sealed class RemoveSpecSingleDto
{
    public RemoveSpecSingleDto( SingleSpecLookupType specType, int specId )
    {
        SpecType = specType;
        SpecId = specId;
    }
    
    public SingleSpecLookupType SpecType { get; }
    public int SpecId { get; }
}