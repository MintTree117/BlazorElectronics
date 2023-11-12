namespace BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

public sealed class UpdateSpecSingleRequest : AddSpecSingleRequest
{
    public UpdateSpecSingleRequest( SingleSpecLookupType specType, int specId, string? specName, Dictionary<int, object>? filterValuesById, List<int>? primaryCategories, bool? isGlobal )
        : base( specType, specName, filterValuesById, primaryCategories, isGlobal )
    {
        SpecId = specId;
    }

    public int SpecId { get; }
}