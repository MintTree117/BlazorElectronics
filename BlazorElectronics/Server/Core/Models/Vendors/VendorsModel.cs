using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Core.Models.Vendors;

public sealed class VendorsModel
{
    public IEnumerable<VendorDto>? Vendors { get; set; }
    public IEnumerable<VendorCategoryModel>? Categories { get; set; }
}