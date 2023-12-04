using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Models.SpecLookups;

public sealed class SpecLookupCategoryModel
{
    public PrimaryCategory PrimaryCategoryId { get; set; }
    public int SpecId { get; set; }
}