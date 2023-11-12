namespace BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

public sealed class DeleteSpecSingleRequest : AdminRequest
{
    public SingleSpecLookupType SpecType { get; set; }
    public int SpecId { get; set; }
}