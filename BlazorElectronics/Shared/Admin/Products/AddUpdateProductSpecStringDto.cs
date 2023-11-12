namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductSpecStringDto
{
    public AddUpdateProductSpecStringDto( int specId, int valueId )
    {
        SpecId = specId;
        ValueId = valueId;
    }
    
    public int SpecId { get; }
    public int ValueId { get; }
}