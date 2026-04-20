using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Departments;

public class Department
{
    private readonly List<DepartmentLocation> _locations;

    private readonly List<DepartmentPosition> _departmentPositions;

    private readonly List<Department> _childDepartments = [];

    private Department(
        Guid? id,
        DepartmentName name,
        DepartmentIndentifier identifier,
        DepartmentPath path,
        short depth, Guid? parentId,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        Path = path;
        Depth = depth;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
        ParentId = parentId;
        _locations = locations.ToList();
        _departmentPositions = positions.ToList();
    }

    private Department()
    {
    }

    public Guid Id { get; }

    public DepartmentName Name { get; private set; }

    public DepartmentIndentifier Identifier { get; private set; }

    public DepartmentPath Path { get; private set; }

    public short Depth { get; private set; }

    public Guid? ParentId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyList<Department> ChildDepartments => _childDepartments;

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _locations;

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;


    public static Result<Department, string> CreateRoot(
        IEnumerable<DepartmentPosition> positions,
        IEnumerable<DepartmentLocation> locations,
        DepartmentName name,
        DepartmentIndentifier indentifier)
    {
        var pathResult = DepartmentPath.Create(indentifier.Identifier);

        if (pathResult.IsFailure) return pathResult.Error;

        return Create(
            positions, locations,
            name, indentifier,
            pathResult.Value,
            parentId: null,
            depth: 0);
    }

    private static Result<Department, string> Create(
        IEnumerable<DepartmentPosition> positions,
        IEnumerable<DepartmentLocation> locations,
        DepartmentName departmentName,
        DepartmentIndentifier departmentIndentifier,
        DepartmentPath path,
        Guid? parentId,
        short depth)
    {
        var department = new Department(
            Guid.NewGuid(),
            departmentName,
            departmentIndentifier,
            path,
            depth,
            parentId,
            locations,
            positions);


        return department;
    }


    public static Result<Department, string> CreateChild(
        IEnumerable<DepartmentPosition> positions,
        IEnumerable<DepartmentLocation> locations,
        DepartmentName name,
        DepartmentIndentifier indentifier,
        Department parent)
    {
        if (parent is null) return "Родительский департамент обязателен";

        var pathResult = DepartmentPath.AddChildToPath(parent.Path.Path, indentifier.Identifier);
        if (pathResult.IsFailure) return pathResult.Error;

        var childDepth = (short)(parent.Depth + 1);

        var childResult = Create(positions, locations,
            name,
            indentifier,
            pathResult.Value,
            parent.Id,
            childDepth);

        if (childResult.IsFailure) return childResult.Error;

        parent.AttachChildToParent(childResult.Value);

        return childResult;
    }

    private UnitResult<string> AttachChildToParent(Department child)
    {
        if (child is null)
            return UnitResult.Failure("Дочерний департамент не может быть null");

        if (_childDepartments.Any(item => item.Identifier == child.Identifier))
            return UnitResult.Failure("Дочерний департамент уже привязан.");

        _childDepartments.Add(child);
        UpdatedAt = DateTimeOffset.UtcNow;
        return UnitResult.Success<string>();
    }

    public UnitResult<string> AddLocation(DepartmentLocation location)
    {
        if (location is null)
            return UnitResult.Failure("Locations обязательно для заполнения");

        if (_locations.Any(l => l.LocationId == location.LocationId))
            return UnitResult.Failure("Location уже существует");

        _locations.Add(location);
        UpdatedAt = DateTimeOffset.UtcNow;

        return UnitResult.Success<string>();
    }


    public UnitResult<string> AddLocations(IEnumerable<DepartmentLocation> locations)
    {
        if (locations is null)
            return UnitResult.Failure("Locations обязательно для заполнения");

        var list = locations
            .Where(a => a is not null)
            .GroupBy(a => a.DepartmentsLocationId)
            .Select(a => a.First())
            .ToList();

        if (list.Count == 0) return UnitResult.Failure("Нет локаций для добавления");

        foreach (var location in list)
        {
            var locationResult = AddLocation(location);
            if (locationResult.IsFailure) return locationResult;
        }

        return UnitResult.Success<string>();
    }

    public UnitResult<string> ReplaceLocations(IEnumerable<DepartmentLocation>? location)
    {
        if (location is null)
            return UnitResult.Failure("Location не может быть null");

        var locationsToList = location.ToList();

        if (locationsToList.Count == 0)
            return UnitResult.Failure("Требуется как минимум одна локация.");

        var backup = _locations.ToList();
        _locations.Clear();

        var replaceResult = AddLocations(locationsToList);

        if (replaceResult.IsFailure)
        {
            _locations.AddRange(backup);
            return replaceResult;
        }

        return UnitResult.Success<string>();
    }
}