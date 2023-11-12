namespace BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

public sealed class UpdateSpecMultiTableRequest : AddSpecMultiTableRequest
{
    public int TableId { get; set; }
}