namespace BlazorElectronics.Server.Models.SpecLookups;

public class SpecLookupValueModel
{
    public int SpecId { get; set; }
    public int SpecValueId { get; set; }
    public string SpecValue { get; set; } = string.Empty;
}