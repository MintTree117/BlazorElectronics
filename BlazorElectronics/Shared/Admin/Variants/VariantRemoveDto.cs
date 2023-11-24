namespace BlazorElectronics.Shared.Admin.Variants;

public sealed class VariantRemoveDto
{
    public VariantRemoveDto()
    {
        
    }
    public VariantRemoveDto( int id )
    {
        VariantId = id;
    }
    
    public int VariantId { get; set; }
}