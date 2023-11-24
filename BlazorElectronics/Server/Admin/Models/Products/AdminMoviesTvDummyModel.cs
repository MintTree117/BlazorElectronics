namespace BlazorElectronics.Server.Admin.Models.Products;

public sealed class AdminMoviesTvDummyModel : AdminProductDummyModel
{
    public string Director { get; set; } = string.Empty;
    public string Cast { get; set; } = string.Empty;
}