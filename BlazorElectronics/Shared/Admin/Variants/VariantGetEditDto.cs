namespace BlazorElectronics.Shared.Admin.Variants;

public sealed class VariantGetEditDto
{
    public VariantGetEditDto()
    {
        
    }

    public VariantGetEditDto( int id )
    {
        VariantId = id;
    }
    
    public int VariantId { get; set; }
}