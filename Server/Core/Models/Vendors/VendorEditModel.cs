using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Core.Models.Vendors;

public sealed class VendorEditModel
{
    public VendorDto? Vendor { get; set; }
    public IEnumerable<VendorCategoryModel>? Categories { get; set; }
}