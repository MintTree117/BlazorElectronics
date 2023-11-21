namespace BlazorElectronics.Shared.Admin.Specs;

public sealed class AddSpecLookupDto
{
    public int SpecId { get; set; }
    public EditSpecLookupDto EditDto { get; set; } = new();
}