namespace BlazorElectronics.Shared.Vendors;

public sealed class Vendor : CrudView
{
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public string VendorUrl { get; init; } = string.Empty;
}