using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface ICategoryService
{
    // USER
    Task<ServiceReply<List<int>?>> GetPrimaryIds(); 
    Task<ServiceReply<CategoryDataDto?>> GetData();
    Task<ServiceReply<List<CategoryLightDto>?>> GetDtos();
    Task<ServiceReply<int>> ValidateCategoryUrl( string url );

    // ADMIN
    Task<ServiceReply<List<CategoryViewDtoDto>?>> GetView();
    Task<ServiceReply<CategoryEditDto?>> GetEdit( int categoryId );
    Task<ServiceReply<bool>> AddBulk( List<CategoryEditDto> categories );
    Task<ServiceReply<int>> Add( CategoryEditDto dto );
    Task<ServiceReply<bool>> Update( CategoryEditDto dto );
    Task<ServiceReply<bool>> Remove( int categoryId );
}