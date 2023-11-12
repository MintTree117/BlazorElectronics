using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductDto
{
    public AddUpdateProductDto( 
        int? productId, AddUpdateProductDetailsDto? details, string? description, List<CategoryIdMap>? categories, List<AddUpdateProductVariantDto>? variants, List<AddUpdateProductImagesDto>? images, AddUpdateProductSpecsDto? specs )
    {
        ProductId = productId;
        Details = details;
        Description = description;
        Categories = categories;
        Variants = variants;
        Images = images;
        Specs = specs;
    }

    public int? ProductId { get; }
    public AddUpdateProductDetailsDto? Details { get; }
    public string? Description { get; }
    public List<CategoryIdMap>? Categories { get; }
    public List<AddUpdateProductVariantDto>? Variants { get; }
    public List<AddUpdateProductImagesDto>? Images { get; }
    public AddUpdateProductSpecsDto? Specs { get; }
}