using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Admin.Variants;

public sealed class VariantViewDto
{
    public PrimaryCategory PrimaryCategory { get; set; }
    public int VariantId { get; set; }
    public string VariantName { get; set; } = string.Empty;
}