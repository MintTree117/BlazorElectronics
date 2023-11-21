using BlazorElectronics.Shared.Inbound.Users;

namespace BlazorElectronics.Shared.Admin;

public class AdminRequest<T>
{
    public AdminRequest()
    {
        
    }
    
    public AdminRequest( UserApiRequest request, T dto )
    {
        SessionApiRequest = request;
        Dto = dto;
    }
    
    public UserApiRequest? SessionApiRequest { get; init; }
    public T? Dto { get; init; }
}