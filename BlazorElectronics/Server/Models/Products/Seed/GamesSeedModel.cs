namespace BlazorElectronics.Server.Models.Products.Seed;

public sealed class GamesSeedModel : ProductSeedModel
{
    public string Developer { get; set; } = string.Empty;
    public bool HasMultiplayer { get; set; }
    public bool HasOfflineMode { get; set; }
    public bool HasControllerSupport { get; set; }
    public bool HasInGamePurchases { get; set; }
    public int FileSizeMb { get; set; }
    
    public List<int> OsRequirements { get; set; } = new();
    public List<int> MultiplayerDetails { get; set; } = new();
}