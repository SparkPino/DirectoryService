namespace Shared;

public record Pagination
{
    public int Page { get; set; }

    public int PageSize { get; set; }
}