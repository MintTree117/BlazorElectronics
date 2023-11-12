namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductSpecsDto
{
    public AddUpdateProductSpecsDto( List<AddUpdateProductSpecIntDto>? intSpecs, List<AddUpdateProductSpecStringDto>? stringSpecs, List<AddUpdateProductSpecBoolDto>? boolSpecs, List<AddUpdateProductSpecMultiDto>? multiSpecs, List<AddUpdateProductSpecsDynamicDto>? dynamicSpecs )
    {
        IntSpecs = intSpecs;
        StringSpecs = stringSpecs;
        BoolSpecs = boolSpecs;
        MultiSpecs = multiSpecs;
        DynamicSpecs = dynamicSpecs;
    }
    
    public List<AddUpdateProductSpecIntDto>? IntSpecs { get; }
    public List<AddUpdateProductSpecStringDto>? StringSpecs { get; }
    public List<AddUpdateProductSpecBoolDto>? BoolSpecs { get; }
    public List<AddUpdateProductSpecMultiDto>? MultiSpecs { get; }
    public List<AddUpdateProductSpecsDynamicDto>? DynamicSpecs { get; }
}