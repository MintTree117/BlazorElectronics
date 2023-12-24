namespace BlazorElectronics.Shared.Reviews;

public sealed class ProductReviewDto
{
    public string Username { get; set; } = string.Empty;
    public string Review { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime Date { get; set; }
}