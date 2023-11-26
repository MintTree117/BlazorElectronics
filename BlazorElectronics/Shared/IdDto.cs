namespace BlazorElectronics.Shared;

public sealed class IdDto
{
    public IdDto()
    {
        
    }
    public IdDto( int id )
    {
        Id = id;
    }
    
    public int Id { get; set; }
}