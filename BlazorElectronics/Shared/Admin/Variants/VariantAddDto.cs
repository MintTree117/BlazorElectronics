namespace BlazorElectronics.Shared.Admin.Variants;

public sealed class VariantAddDto
{
    public VariantAddDto()
    {
        
    }
    public VariantAddDto( VariantEditDto dto )
    {
        PrimaryCategoryId = dto.PrimaryCategoryId;
        VariantName = dto.VariantName;
        VariantValues = dto.VariantValues;
    }

    public int PrimaryCategoryId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string VariantValues { get; set; } = string.Empty;
}