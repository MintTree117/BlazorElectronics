namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductSpecIntDto
{
    public AddUpdateProductSpecIntDto( int specId, int filterId, int value )
    {
        SpecId = specId;
        FilterId = filterId;
        Value = value;
    }
    
    public int SpecId { get; }
    public int FilterId { get; }
    public int Value { get; }
}