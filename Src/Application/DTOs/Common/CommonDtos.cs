namespace prismodSale.Src.Application.DTOs.Common;

public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}

public class SellableProductQueryFilters
{
    public string? Search { get; set; }
    public string? CategoryCen { get; set; }
    public string? WarehouseCen { get; set; }
    public bool OnlyAvailable { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
