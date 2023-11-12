namespace BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

public sealed class DeleteSpecSingleRequest : AdminRequest
{
    public DeleteSpecSingleRequest( SingleSpecLookupType specType, int specId )
    {
        SpecType = specType;
        SpecId = specId;
    }
    
    public SingleSpecLookupType SpecType { get; }
    public int SpecId { get; }
}