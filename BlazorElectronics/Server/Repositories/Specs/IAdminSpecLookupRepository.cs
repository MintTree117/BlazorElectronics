using BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface IAdminSpecLookupRepository
{
    Task<bool> AddSpecSingle( SingleSpecLookupType specType, string specName, Dictionary<int, object>? filterValuesById, List<int>? primaryCategories, bool? isGlobal );
    Task<bool> UpdateSpecSingle( SingleSpecLookupType specType, int specId, string specName, Dictionary<int, object>? filterValuesById, List<int>? primaryCategories, bool? isGlobal );
    Task<bool> DeleteSpecSingle( SingleSpecLookupType specType, int specId );
    
    Task<bool> AddSpecMultiTable( string tableName, List<string>? multiValues, List<int>? primaryCategories, bool? isGlobal );
    Task<bool> UpdateSpecMultiTable( int tableId, string tableName, List<string>? multiValues, List<int>? primaryCategories, bool? isGlobal );
    Task<bool> DeleteSpecMultiTable( int tableId, string tableName );
}