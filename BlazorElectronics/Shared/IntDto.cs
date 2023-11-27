namespace BlazorElectronics.Shared;

public sealed class IntDto
{
    public IntDto()
    {
        
    }
    public IntDto( int value )
    {
        Value = value;
    }
    
    public int Value { get; set; }
}