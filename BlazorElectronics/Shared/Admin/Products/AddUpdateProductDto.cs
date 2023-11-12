using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductDto
{
    public AddUpdateProductDto( 
        AddUpdateProductDetailsDto? details, string? description, List<CategoryIdMap>? categories, List<AddUpdateProductVariantDto>? variants, List<AddUpdateProductImagesDto>? images, AddUpdateProductSpecsDto? specs )
    {
        Details = details;
        Description = description;
        Categories = categories;
        Variants = variants;
        Images = images;
        Specs = specs;
    }

    public AddUpdateProductDetailsDto? Details { get; }
    public string? Description { get; }
    public List<CategoryIdMap>? Categories { get; }
    public List<AddUpdateProductVariantDto>? Variants { get; }
    public List<AddUpdateProductImagesDto>? Images { get; }
    public AddUpdateProductSpecsDto? Specs { get; }
}