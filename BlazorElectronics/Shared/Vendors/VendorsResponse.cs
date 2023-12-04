using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Vendors;

public sealed class VendorsResponse
{
    public Dictionary<PrimaryCategory, List<int>> VendorIdsByCategory { get; set; } = new();
    public Dictionary<int, Vendor> VendorsById { get; set; } = new();
}