using CSharpFunctionalExtensions;

namespace DirectoryService.Domain;

public class Position
{
    private readonly List<DepartmentPosition> _departmentsPositions = [];

    private Position(Guid? id, PositionName name, string? description)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    private Position()
    {
    }

    public Guid Id { get; private set; }

    public PositionName Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> DepartmentsPositions => _departmentsPositions;


    public static Result<Position, string> Create(Guid? id, PositionName name,
        IEnumerable<DepartmentPosition> departments,
        string? description = null)
    {
        if (name == null) return "Имя не может быть Null";

        description = (description ?? string.Empty).Trim();

        if (description.Length > 1000) return "Максимально 1000 символов";

        var position = new Position(id, name, description);


        foreach (var department in departments ?? [])
        {
            var positionResult = position.AddDepartment(department);
            if (positionResult.IsFailure)
                return positionResult.Error;
        }

        return position;
    }

    public UnitResult<string> AddDepartment(DepartmentPosition? department)
    {
        if (department == null) return UnitResult.Failure("Не может быть null");
        if (_departmentsPositions.Any(d => d.DepartmentId == department.DepartmentId))
            return UnitResult.Failure("Департамент уже добавлен");

        _departmentsPositions.Add(department);
        UpdatedAt = DateTimeOffset.UtcNow;
        return UnitResult.Success<string>();
    }
}