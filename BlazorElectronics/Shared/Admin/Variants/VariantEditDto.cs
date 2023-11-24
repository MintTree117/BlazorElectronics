namespace BlazorElectronics.Shared.Admin.Variants;

public sealed class VariantEditDto
{
    public int VariantId { get; set; } = -1;
    public int PrimaryCategoryId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string VariantValues { get; set; } = string.Empty;
}