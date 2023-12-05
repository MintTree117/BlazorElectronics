using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Models.Vendors;

public sealed class VendorCategoryModel
{
    public int VendorId { get; set; }
    public int PrimaryCategoryId { get; set; }
}