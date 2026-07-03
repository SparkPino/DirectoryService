namespace DirectoryService.Application.Abstraction.Repositories;

/// <summary>
/// Плоский рядок результату SQL-запиту по locations: лише примітиви,
/// щоб будь-які LINQ-оператори поверх нього транслювались у SQL.
/// </summary>
public sealed class LocationRow
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public string Addresses { get; init; } = null!;

    public DateTimeOffset CreatedAt { get; init; }

    public int AttachDepartmentCount { get; init; }
}