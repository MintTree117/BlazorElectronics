namespace BlazorElectronics.Server.Models.Specs;

public sealed class DynamicSpecValueModel
{
    public int SpecId { get; set; }
    public string SpecValue { get; set; } = string.Empty;
}