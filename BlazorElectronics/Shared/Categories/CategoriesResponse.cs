namespace BlazorElectronics.Shared.Categories;

public sealed class CategoriesResponse
{
    public CategoriesResponse()
    {
        
    }
    public CategoriesResponse( Dictionary<int, CategoryResponse> categories, List<int> primary )
    {
        CategoriesById = categories;
        PrimaryIds = primary;
    }

    public Dictionary<int, CategoryResponse> CategoriesById { get; init; } = new();
    public List<int> PrimaryIds { get; init; } = new();
}