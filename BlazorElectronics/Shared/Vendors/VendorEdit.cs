using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Vendors;

public sealed class VendorEdit : ICrudEdit
{
    public int VendorId { get; set; }
    public bool IsGlobal { get; set; }
    public List<PrimaryCategory> PrimaryCategories { get; set; } = new();
    public string VendorName { get; set; } = string.Empty;
    public string VendorUrl { get; set; } = string.Empty;
    
    public void SetId( int id )
    {
        VendorId = id;
    }
}