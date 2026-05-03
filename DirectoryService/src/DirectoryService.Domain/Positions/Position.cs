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


    public static Result<Position, string> Create(PositionName name, string? description, PositionId? id = null)
    {
        description = (description ?? string.Empty).Trim();
        if (description.Length > 1000) return "Максимально 1000 символов";

        var position = new Position(id, name, description);

        return position;
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