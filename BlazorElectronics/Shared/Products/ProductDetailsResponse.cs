using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Products;

namespace BlazorElectronics.Shared.Outbound.Products;

public class ProductDetailsResponse
{   
    public int Id { get; init; }

    public ProductCategoryResponse? PrimaryCategory { get; init; }
    public List<ProductCategoryResponse> SecondaryCategories { get; init; } = new();
    public List<ProductCategoryResponse> TertiaryCategories { get; init; } = new();
    
    public decimal Rating { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime ReleaseDate { get; init; }
    public bool HasDrm { get; init; }
    public int NumberSold { get; init; }
    public string VariantTypeName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<ProductImageResponse> Images { get; init; } = new();
    public List<ProductReview_DTO> Reviews { get; init; } = new();
}