namespace SimplePersonalFinance.Shared.Contracts;

public class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalItems { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedResult(List<T> items, int totalItems, int pageNumber, int pageSize)
    {
        Items = items.AsReadOnly();
        TotalItems = totalItems;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
