namespace BlazorElectronics.Server.Admin.Models.Products;

public sealed class AdminProductSoftwareDummyModel : AdminProductDummyModel
{
    public string Version { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Dependencies { get; set; } = string.Empty;
    public string TrialLimitations { get; set; } = string.Empty;
}