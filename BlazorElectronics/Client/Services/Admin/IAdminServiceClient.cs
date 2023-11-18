using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminServiceClient
{
    Task<ApiReply<bool>> AuthorizeAdminView();
}