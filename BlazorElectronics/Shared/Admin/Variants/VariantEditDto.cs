using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Admin.Variants;

public sealed class VariantEditDto
{
    public int VariantId { get; set; } = -1;
    public PrimaryCategory PrimaryCategoryId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string VariantValues { get; set; } = string.Empty;
}