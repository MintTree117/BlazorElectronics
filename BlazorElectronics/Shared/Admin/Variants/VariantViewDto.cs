using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Admin.Variants;

public sealed class VariantViewDto
{
    public int VariantId { get; set; }
    public PrimaryCategory PrimaryCategoryId { get; set; }
    public string VariantName { get; set; } = string.Empty;
}