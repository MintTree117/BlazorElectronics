using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminFeaturedProductsServiceClient : IAdminViewService<AdminItemViewDto>
{
    Task<ServiceReply<FeaturedProductDto?>> GetEdit( IntDto dto );
    Task<ServiceReply<int>> Add( FeaturedProductDto dto );
    Task<ServiceReply<bool>> Update( FeaturedProductDto dto );
}