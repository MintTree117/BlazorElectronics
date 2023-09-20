namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductCategory
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryImageUrl { get; set; } = string.Empty;
}

public enum ProductCategoryEnum
{
    Cpu,
    Gpu,
    Ram,
    Mobo,
    Psu,
    Cooling,
    Cases,
    Displays,
    Keyboards,
    Mice,
    Headphones,
    Speakers,
    Mics,
    Laptops,
    Cables,
    Chargers
}