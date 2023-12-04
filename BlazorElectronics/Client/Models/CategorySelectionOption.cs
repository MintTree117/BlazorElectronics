using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Client.Models;

public sealed class CategorySelectionOption
{
    public PrimaryCategory Category { get; set; }
    public bool IsSelected { get; set; }
}