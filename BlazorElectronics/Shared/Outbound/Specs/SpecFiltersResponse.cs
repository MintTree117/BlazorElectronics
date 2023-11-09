namespace BlazorElectronics.Shared.Outbound.Specs;

public sealed class SpecFiltersResponse
{
    public List<FilterTableResponse> IntFilters { get; set; } = new();
    // table
    //   name
    //   list<intfilters>
    //     id
    //     value
    public List<FilterTableResponse> StringFilters { get; set; } = new();
    // table
    //   name
    //   list<stringfilters>
    //     id
    //     value
    public List<string> BoolFilters { get; set; } = new();
    // bool names
    public List<FilterTableResponse> DynamicFilters { get; set; } = new();
    // table
    //   name
    //   list<dynamicfilters>
    //     id
    //     value
}