namespace BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

public sealed class UpdateSpecSingleRequest : AddSpecSingleRequest
{
    public int SpecId { get; set; }
}