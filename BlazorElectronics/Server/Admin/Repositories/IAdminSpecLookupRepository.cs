using BlazorElectronics.Shared.Admin.SpecsMulti;
using BlazorElectronics.Shared.Admin.SpecsSingle;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminSpecLookupRepository
{
    Task<bool> AddSpecSingle( AddSpecSingleDto dto );
    Task<bool> UpdateSpecSingle( UpdateSpecSingleDto dto );
    Task<bool> RemoveSpecSingle( RemoveSpecSingleDto dto );
    
    Task<bool> AddSpecMultiTable( AddSpecMultiDto dto );
    Task<bool> UpdateSpecMultiTable( UpdateSpecMultiDto dto );
    Task<bool> RemoveSpecMultiTable( RemoveSpecMultiDto dto );
}