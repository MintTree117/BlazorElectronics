namespace BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

public sealed class DeleteSpecMultiTableRequest : AdminRequest
{
    public string? TableName { get; set; }
}