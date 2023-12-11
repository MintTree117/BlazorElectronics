using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Client.Models;

public sealed class CategorySeed
{
    public int PrimaryId { get; set; }
    public int ParentCategoryId { get; set; }
    public CategoryTier Tier { get; set; }
    public string Names { get; set; } = string.Empty;
}