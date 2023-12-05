namespace BlazorElectronics.Shared.Vendors;

public sealed class VendorsResponse
{
    public Dictionary<int, List<int>> VendorIdsByCategory { get; set; } = new();
    public Dictionary<int, Vendor> VendorsById { get; set; } = new();
}