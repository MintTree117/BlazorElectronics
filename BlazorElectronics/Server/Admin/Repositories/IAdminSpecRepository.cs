using BlazorElectronics.Shared.Admin.SpecsMulti;
using BlazorElectronics.Shared.Admin.SpecsSingle;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminSpecRepository
{
    Task<bool> AddSpecSingle( AddUpdateSpecSingleDto dto );
    Task<bool> UpdateSpecSingle( AddUpdateSpecSingleDto dto );
    Task<bool> RemoveSpecSingle( RemoveSpecSingleDto dto );
    
    Task<bool> UpdateSpecMultiTable( UpdateSpecMultiDto dto );
}