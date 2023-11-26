using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Admin.Models.Variants;

public sealed class VariantsModel
{
    public Dictionary<PrimaryCategory, int> VariantsByCategory { get; set; } = new();
    public Dictionary<int, List<int>> VariantValuesById { get; set; } = new();
}