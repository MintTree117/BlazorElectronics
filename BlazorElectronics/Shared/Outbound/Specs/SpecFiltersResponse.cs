namespace BlazorElectronics.Shared.Outbound.Specs;

public sealed class SpecFiltersResponse
{
    public List<SpecFilterTableResponse> IntFilters { get; set; } = new();
    // table
    //   name
    //   list<intfilters>
    //     id
    //     value
    public List<SpecFilterTableResponse> StringFilters { get; set; } = new();
    // table
    //   name
    //   list<stringfilters>
    //     id
    //     value
    public List<string> BoolFilters { get; set; } = new();
    // bool names
    public List<SpecFilterTableResponse> MultiFilters { get; set; } = new();
    // table
    //   name
    //   list<dynamicfilters>
    //     id
    //     value
}