namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductDetailsDto
{
    public AddUpdateProductDetailsDto( int variantTypeId, string? title, string? thumbnail, DateTime? releaseDate, bool hasDrm )
    {
        VariantTypeId = variantTypeId;
        Title = title;
        Thumbnail = thumbnail;
        ReleaseDate = releaseDate;
        HasDrm = hasDrm;
    }
    
    public int VariantTypeId { get; }
    public string? Title { get; }
    public string? Thumbnail { get; }
    public DateTime? ReleaseDate { get; }
    public bool HasDrm { get; }
}