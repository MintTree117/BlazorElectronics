namespace BlazorElectronics.Shared.Categories;

public sealed class CategoriesViewDto
{
    public CategoriesViewDto( List<CategoryViewDto> views )
    {
        Views = views;
    }

    public List<CategoryViewDto> Views { get; set; } = new();
}