namespace BlazorElectronics.Shared.Outbound.Specs;

public sealed class FilterTableResponse
{
    public FilterTableResponse( int id, string name, IReadOnlyList<string> values )
    {
        Id = id;
        Name = name;
        Values = values;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyList<string> Values { get; set; }
}