using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Positions;

public sealed class Position
{
    private readonly List<DepartmentPosition> _departmentsPositions = [];

    private Position(PositionId? id, PositionName name, string? description)
    {
        Id = id ?? new PositionId(Guid.NewGuid());
        Name = name;
        Description = description;
        CreatedAt = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    private Position()
    {
    }

    public PositionId Id { get; private set; } = null!;

    public PositionName Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> DepartmentsPositions => _departmentsPositions;


    public static Result<Position, Error> Create(PositionName name, string? description, PositionId? id = null)
    {
        description = (description ?? string.Empty).Trim();
        if (description.Length > 1000)
            return Error.Validation("position.description.length", "Максимально 1000 символов");

        var position = new Position(id, name, description);

        return position;
    }

    public UnitResult<Errors> Rename(PositionName? name = null, string? description = null)
    {
        if (name is null && description is null)
            return Error.Validation("update.position", "нужно указать хотя бы одно поле").ToErrors();

        description = description?.Trim();
        if (description is not null && description.Length > 1000)
            return Error.Validation("position.description.length", "Максимально 1000 символов").ToErrors();

        Name = name ?? Name;
        Description = description ?? Description;
        UpdatedAt = DateTimeOffset.UtcNow;

        return UnitResult.Success<Errors>();
    }

    public UnitResult<Error> AddDepartment(DepartmentId departmentId)
    {
        if (_departmentsPositions.Any(d => d.DepartmentId == departmentId))
            return Error.Conflict("department.already.exist", "Департамент уже добавлен");

        var departmentLocation = new DepartmentPosition(departmentId, Id);

        _departmentsPositions.Add(departmentLocation);
        UpdatedAt = DateTimeOffset.UtcNow;

        return UnitResult.Success<Error>();
    }
}