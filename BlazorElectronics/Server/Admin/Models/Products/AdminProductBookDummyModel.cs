namespace BlazorElectronics.Server.Admin.Models.Products;

public sealed class AdminProductBookDummyModel : AdminProductDummyModel
{
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string? Narrator { get; set; } = null;
    public string ISBN { get; set; } = string.Empty;
}