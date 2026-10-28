namespace DirectoryService.Contracts.Search;

public sealed record SearchResultItemDto
{
    public string Type { get; init; } = null!;

    public Guid Id { get; init; }

    public string Title { get; init; } = null!;

    public string? Subtitle { get; init; }
}