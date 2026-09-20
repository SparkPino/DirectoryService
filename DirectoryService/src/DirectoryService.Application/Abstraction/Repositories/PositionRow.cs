namespace DirectoryService.Application.Abstraction.Repositories;

/// <summary>
/// Плоский рядок результату SQL-запиту по positions: лише примітиви,
/// щоб будь-які LINQ-оператори поверх нього транслювались у SQL.
/// </summary>
public sealed class PositionRow
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public int AttachDepartmentCount { get; init; }
}
