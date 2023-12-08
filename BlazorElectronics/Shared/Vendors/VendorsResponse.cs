namespace BlazorElectronics.Shared.Vendors;

public sealed class VendorsResponse
{
    public Dictionary<int, List<int>> VendorIdsByCategory { get; init; } = new();
    public Dictionary<int, VendorModel> VendorsById { get; init; } = new();
}