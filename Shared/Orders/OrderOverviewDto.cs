namespace BlazorElectronics.Shared.Orders;

public sealed record OrderOverviewDto
{
    public DateTime OrderDate { get; init; }
    public decimal TotalPrice { get; init; }
    public int NumberItems { get; init; }
}