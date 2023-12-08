using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Models.Vendors;

public sealed class VendorsModel
{
    public IEnumerable<VendorModel>? Vendors { get; set; }
    public IEnumerable<VendorCategoryModel>? Categories { get; set; }
}