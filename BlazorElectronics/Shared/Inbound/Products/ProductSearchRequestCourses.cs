namespace BlazorElectronics.Shared.Inbound.Products;

public sealed class ProductSearchRequestCourses : ProductSearchRequest
{
    public int? MinNumLectures { get; set; }
    public int? MaxNumLectures { get; set; }
    public int? MinHoursPerWeek { get; set; }
    public int? MxHoursPerWeek { get; set; }
    public int? MinCourseDuration { get; set; }
    public int? MaxCourseDuration { get; set; }

    public Dictionary<int, List<int>>? CoursesDynamicFiltersInclude { get; set; }
    public Dictionary<int, List<int>>? CoursesDynamicFiltersExclude { get; set; }
}