using BlazorElectronics.Shared.Inbound.Users;

namespace BlazorElectronics.Shared.Admin;

public class AdminRequest<T>
{
    public AdminRequest( SessionApiRequest request, T dto )
    {
        SessionApiRequest = request;
        Dto = dto;
    }
    
    public SessionApiRequest? SessionApiRequest { get; }
    public T? Dto { get; }
}