using BlazorElectronics.Shared.Admin.Products;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminProductRepository
{
    Task<bool> AddProduct( AddUpdateProductDto dto );
    Task<bool> UpdateProduct( AddUpdateProductDto dto );
    Task<bool> RemoveProduct( RemoveProductDto dto );
}