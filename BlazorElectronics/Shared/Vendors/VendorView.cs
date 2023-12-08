namespace BlazorElectronics.Shared.Vendors;

public sealed class VendorView : CrudView
{
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
}