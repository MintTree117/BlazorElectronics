namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductSpecBoolDto
{
    public AddUpdateProductSpecBoolDto( int specId, bool value )
    {
        SpecId = specId;
        Value = value;
    }
    
    public int SpecId { get; }
    public bool Value { get; }
}