using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Departments;

public class Department
{
    private readonly List<DepartmentsLocation> _locations;

    private readonly List<DepartmentPosition> _positions;

    private readonly List<Department> _childDepartments = new();

    private Department(
        Guid? id,
        DepartmentName departmentName,
        DepartmentIndentifier departmentIndentifier,
        DepartmentPath path,
        short depth, Guid? parentId,
        IEnumerable<DepartmentsLocation> locations,
        IEnumerable<DepartmentPosition> positions)
    {
        Id = id ?? Guid.NewGuid();
        DepartmentName = departmentName;
        DepartmentIndentifier = departmentIndentifier;
        DepartmentPath = path;
        Depth = depth;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        ParentId = parentId;
        _locations = locations.ToList();
        _positions = positions.ToList();
    }

    private Department()
    {
    }

    public Guid Id { get; }

    public DepartmentName DepartmentName { get; private set; }

    public DepartmentIndentifier DepartmentIndentifier { get; private set; }

    public DepartmentPath DepartmentPath { get; private set; }

    public short Depth { get; private set; }

    public Guid? ParentId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<Department> ChildDepartments => _childDepartments;

    public IReadOnlyList<DepartmentsLocation> Location => _locations;

    public IReadOnlyList<DepartmentPosition> Positions => _positions;


    public static Result<Department, string> CreateRoot(
        IEnumerable<DepartmentPosition> positions,
        IEnumerable<DepartmentsLocation> locations,
        DepartmentName departmentName,
        DepartmentIndentifier departmentIndentifier)
    {
        var pathResult = DepartmentPath.Create(departmentIndentifier.Value);

        if (pathResult.IsFailure) return pathResult.Error;

        return Create(
            positions, locations,
            departmentName, departmentIndentifier,
            pathResult.Value,
            parentId: null,
            depth: 0);
    }

    private static Result<Department, string> Create(
        IEnumerable<DepartmentPosition> positions,
        IEnumerable<DepartmentsLocation> locations,
        DepartmentName departmentName,
        DepartmentIndentifier departmentIndentifier,
        DepartmentPath path,
        Guid? parentId,
        short depth)
    {
        if (departmentName is null) return "DepartmentName обязательно для заполнения";
        if (departmentIndentifier is null) return "Positions обязательно для заполнения";
        if (positions is null) return "Поле Positions обязательно для заполнения";
        if (locations is null) return "Поле Locations обязательно для заполнения";


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
        IEnumerable<DepartmentsLocation> locations,
        DepartmentName departmentName,
        DepartmentIndentifier departmentIndentifier,
        Department parent)
    {
        if (parent is null) return "Родительский департамент обязателен";

        var pathResult = DepartmentPath.AddChildToPath(parent.DepartmentPath.Value, departmentIndentifier.Value);
        if (pathResult.IsFailure) return pathResult.Error;

        var childDepth = (short)(parent.Depth + 1);

        var childResult = Create(positions, locations,
            departmentName,
            departmentIndentifier,
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

        if (_childDepartments.Any(item => item.DepartmentIndentifier == child.DepartmentIndentifier))
            return UnitResult.Failure("Дочерний департамент уже привязан.");

        _childDepartments.Add(child);
        UpdatedAt = DateTime.UtcNow;
        return UnitResult.Success<string>();
    }

    public UnitResult<string> AddLocation(DepartmentsLocation location)
    {
        if (location is null)
            return UnitResult.Failure("Locations обязательно для заполнения");

        if (_locations.Any(l => l.Id == location.Id))
            return UnitResult.Failure("Location уже существует");

        _locations.Add(location);
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<string>();
    }


    public UnitResult<string> AddLocations(IEnumerable<DepartmentsLocation> locations)
    {
        if (locations is null)
            return UnitResult.Failure("Locations обязательно для заполнения");

        var list = locations
            .Where(a => a is not null)
            .GroupBy(a => a.Id)
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

    public UnitResult<string> ReplaceLocations(IEnumerable<DepartmentsLocation> location)
    {
        if (location is null) return UnitResult.Failure("Location обязательно для заполнения");

        var locationsToList = location.ToList();

        if (locationsToList.Count == 0) return UnitResult.Failure("Требуется как минимум одна локация.");

        _locations.Clear();

        var ReplaceResult = AddLocations(locationsToList);

        if (ReplaceResult.IsFailure) return ReplaceResult;

        return UnitResult.Success<string>();
    }
}