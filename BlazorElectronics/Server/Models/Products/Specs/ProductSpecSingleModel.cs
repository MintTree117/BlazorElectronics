namespace BlazorElectronics.Server.Models.Products.Specs;

public sealed class ProductSpecSingleModel
{
    public ProductSpecSingleModel( string specName, object specValue )
    {
        SpecName = specName;
        SpecValue = specValue;
    }
    
    public string SpecName { get; }
    public object SpecValue { get; }
}