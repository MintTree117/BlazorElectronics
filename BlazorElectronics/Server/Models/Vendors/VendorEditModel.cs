using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Models.Vendors;

public sealed class VendorEditModel
{
    public VendorModel? Vendor { get; set; }
    public IEnumerable<VendorCategoryModel>? Categories { get; set; }
}