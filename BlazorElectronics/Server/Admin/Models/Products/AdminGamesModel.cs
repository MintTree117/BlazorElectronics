namespace BlazorElectronics.Server.Admin.Models.Products;

public sealed class AdminGamesModel : AdminProductModel
{
    public string Developer { get; set; } = string.Empty;
    public bool HasMultiplayer { get; set; }
    public bool HasOfflineMode { get; set; }
    public bool HasControllerSupport { get; set; }
    public bool HasInGamePurchases { get; set; }

    public List<int> MultiplayerDetails { get; set; } = new();
}