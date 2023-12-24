namespace BlazorElectronics.Shared;

public sealed class PaginationDto
{
    public PaginationDto()
    {
        
    }
    public PaginationDto( int rows, int page )
    {
        Rows = rows;
        Page = page;
    }
    
    public int Rows { get; set; }
    public int Page { get; set; }
}