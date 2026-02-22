using CSharpFunctionalExtensions;

namespace DirectoryService.Domain;

public class Position
{
    private readonly List<DepartmentPosition> _departments;

    private Position(Guid? id, PositionName name, string? description, IEnumerable<DepartmentPosition> departments)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        _departments = departments.ToList();
        IsActive = true;
    }

    private Position()
    {
    }

    public Guid Id { get; private set; }

    public PositionName Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> Departments => _departments;


    public static Result<Position, string> Create(Guid? id, PositionName name,
        IEnumerable<DepartmentPosition> departments,
        string? description = null)
    {
        if (name == null) return "Имя не может быть Null";

        description = (description ?? string.Empty).Trim();

        if (description.Length > 1000) return "Максимально 1000 символов";

        var position = new Position(id, name, description, departments);

        if (departments != null)
        {
            foreach (var department in departments)
            {
                var positionResult = position.AddDepartment(department);
                if (positionResult.IsFailure)
                    return positionResult.Error;
            }
        }


        return position;
    }

    public UnitResult<string> AddDepartment(DepartmentPosition department)
    {
        if (department == null) return UnitResult.Failure("Не может быть null");
        if (_departments.Any(d => d.DepartmentId == department.DepartmentId))
            return UnitResult.Failure("Департамент уже добавлен");

        _departments.Add(department);
        UpdatedAt = DateTime.UtcNow;
        return UnitResult.Success<string>();
    }
}