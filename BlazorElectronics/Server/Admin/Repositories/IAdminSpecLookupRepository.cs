using BlazorElectronics.Shared.Admin.SpecsMulti;
using BlazorElectronics.Shared.Admin.SpecsSingle;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminSpecLookupRepository
{
    Task<bool> AddSpecSingle( AddUpdateSpecSingleDto dto );
    Task<bool> UpdateSpecSingle( AddUpdateSpecSingleDto dto );
    Task<bool> RemoveSpecSingle( RemoveSpecSingleDto dto );
    
    Task<bool> AddSpecMultiTable( AddUpdateSpecMultiDto dto );
    Task<bool> UpdateSpecMultiTable( AddUpdateSpecMultiDto dto );
    Task<bool> RemoveSpecMultiTable( RemoveSpecMultiDto dto );
}