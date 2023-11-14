namespace BlazorElectronics.Server.Models.Products.Specs;

public sealed class ProductSpecMultiModel
{
    public ProductSpecMultiModel( string tableName, List<string> specValues )
    {
        TableName = tableName;
        SpecValues = specValues;
    }
    
    public string TableName { get; }
    public List<string> SpecValues { get; }
}