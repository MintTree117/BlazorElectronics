using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminServiceClient
{
    Task<ServiceReply<bool>> AuthorizeAdmin();
}