namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductCategory
{
    public int ProductCategoryId { get; set; }
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
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