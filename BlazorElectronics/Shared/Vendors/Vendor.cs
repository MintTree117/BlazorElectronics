namespace BlazorElectronics.Shared.Vendors;

public sealed class Vendor : CrudView
{
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string VendorUrl { get; set; } = string.Empty;
}