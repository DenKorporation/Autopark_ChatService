namespace ChatService.IntegrationTests.Responses;

public class PagedList<T>
{
    public List<T> Items { get; init; } = null!;

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public bool HasNextPage { get; init; }

    public bool HasPreviousPage { get; init; }
}
