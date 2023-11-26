namespace BlazorElectronics.Server.Admin.Models.Products;

public sealed class AdminSoftwareModel : AdminProductModel
{
    public string Version { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Dependencies { get; set; } = string.Empty;
    public string TrialLimitations { get; set; } = string.Empty;

    public List<int> OsRequirements { get; set; } = new();
}