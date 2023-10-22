using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users;

public class UserServiceClient : IUserServiceClient
{
    readonly HttpClient _http;
    
    public UserServiceClient( HttpClient http )
    {
        _http = http;
    }

    public async Task<ServiceResponse<int>> Register( UserRegister_DTO request )
    {
        try
        {
            HttpResponseMessage result = await _http.PostAsJsonAsync( "api/User/register", request );
            return ( await result.Content.ReadFromJsonAsync<ServiceResponse<int>>() )!;
        }
        catch ( Exception e )
        {
            return new ServiceResponse<int>( -1, false, e.Message );
        }

    }
    public async Task<ServiceResponse<UserLoginResponse_DTO?>> Login( UserLoginRequest_DTO request )
    {
        try
        {
            HttpResponseMessage result = await _http.PostAsJsonAsync( "api/User/login", request );
            return ( await result.Content.ReadFromJsonAsync<ServiceResponse<UserLoginResponse_DTO>>() )!;
        }
        catch ( Exception e )
        {
            return new ServiceResponse<UserLoginResponse_DTO?>( null, false, e.Message );
        }
    }
}