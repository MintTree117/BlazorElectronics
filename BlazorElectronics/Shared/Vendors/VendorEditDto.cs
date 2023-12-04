namespace BlazorElectronics.Shared.Vendors;

public sealed class VendorEditDto
{
    public int VendorId { get; set; }
    public bool IsGlobal { get; set; }
    public string PrimaryCategories { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorUrl { get; set; } = string.Empty;
}