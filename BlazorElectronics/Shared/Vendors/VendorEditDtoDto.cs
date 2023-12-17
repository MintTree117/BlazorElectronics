namespace BlazorElectronics.Shared.Vendors;

public sealed class VendorEditDtoDto : ICrudEditDto
{
    public int VendorId { get; set; }
    public bool IsGlobal { get; set; }
    public List<int> PrimaryCategories { get; set; } = new();
    public string VendorName { get; set; } = string.Empty;
    public string VendorUrl { get; set; } = string.Empty;
    
    public void SetId( int id )
    {
        VendorId = id;
    }
}