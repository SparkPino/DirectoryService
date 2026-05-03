using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    private readonly List<DepartmentLocation> _departmentsLocations = [];

    private readonly List<DepartmentPosition> _positions = [];

    private readonly List<Department> _childDepartments = [];

    private Department(
        DepartmentId id,
        DepartmentName name,
        DepartmentIdentifier identifier,
        DepartmentPath path,
        short depth,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions,
        DepartmentId? parentId = null)
    {
        Id = id;
        Name = name;
        Identifier = identifier;
        Path = path;
        Depth = depth;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        ParentId = parentId;
        _departmentsLocations = locations.ToList();
        _positions = positions.ToList();
    }

    private Department()
    {
    }

    public DepartmentId Id { get; private set; } = null!;

    public DepartmentName Name { get; private set; } = null!;

    public DepartmentIdentifier Identifier { get; private set; } = null!;

    public DepartmentPath Path { get; private set; } = null!;

    public DepartmentId? ParentId { get; private set; }

    public short Depth { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyList<Department> ChildDepartments => _childDepartments;

    public IReadOnlyList<DepartmentLocation> DepartmentsLocations => _departmentsLocations;

    public IReadOnlyList<DepartmentPosition> Positions => _positions;


    public static Result<Department, Errors> CreateRoot(
        IEnumerable<DepartmentPosition> positions,
        IEnumerable<DepartmentLocation> locations,
        DepartmentName name,
        DepartmentIdentifier identifier,
        DepartmentId? departmentId = null)
    {
        const int depth = 0;

        var locationsDepartmentList = locations.ToList();

        if (locationsDepartmentList.Count == 0)
        {
            return Error.Validation("department.locations", "Department location должно содержать хотябы одну локацию")
                .ToErrors();
        }

        var pathResult = DepartmentPath.CreateParent(identifier);
        if (pathResult.IsFailure) return pathResult.Error;

        return Create(
            positions, locationsDepartmentList,
            name, identifier,
            pathResult.Value,
            depth,
            departmentId);
    }

    private static Department Create(
        IEnumerable<DepartmentPosition> positions,
        IEnumerable<DepartmentLocation> locations,
        DepartmentName departmentName,
        DepartmentIdentifier departmentIdentifier,
        DepartmentPath path,
        short depth,
        DepartmentId? parentId = null)
    {
        var department = new Department(
            new DepartmentId(Guid.NewGuid()),
            departmentName,
            departmentIdentifier,
            path,
            depth,
            locations,
            positions,
            parentId);

        return department;
    }

    public Result<Department, Errors> CreateChild(
        IEnumerable<DepartmentPosition> positions,
        IEnumerable<DepartmentLocation> locations,
        DepartmentName name,
        DepartmentIdentifier identifier)
    {
        var departmentLocationList = locations.ToList();
        if (departmentLocationList.Count == 0)
        {
            return Error.Validation("department.locations", "Department location должно содержать хотябы одну локацию")
                .ToErrors();
        }

        var childPath = Path.CreateChildPath(identifier);
        if (childPath.IsFailure) return childPath.Error;

        short childDepth = (short)(Depth + 1);

        var createDepartmentChild = Create(
            positions,
            departmentLocationList,
            name,
            identifier,
            childPath.Value,
            childDepth,
            Id);

        var attachChildToParentResult = AttachChildToParent(createDepartmentChild);
        if (attachChildToParentResult.IsFailure) return attachChildToParentResult.Error.ToErrors();

        return createDepartmentChild;
    }

    public static Result<Department, Errors> CreateChild(
        IEnumerable<DepartmentPosition> positions,
        IEnumerable<DepartmentLocation> locations,
        DepartmentName name,
        DepartmentIdentifier identifier,
        Department parent)
    {
        var departmentLocationList = locations.ToList();
        if (departmentLocationList.Count == 0)
        {
            return Error.Validation("department.locations", "Department location должно содержать хотябы одну локацию")
                .ToErrors();
        }

        var childPath = parent.Path.CreateChildPath(identifier);
        if (childPath.IsFailure) return childPath.Error;
        short childDepth = (short)(parent.Depth + 1);

        var createDepartmentChild = Create(
            positions,
            departmentLocationList,
            name,
            identifier,
            childPath.Value,
            childDepth,
            parent.Id);

        var attachChildToParentResult = AttachChildToParent(createDepartmentChild, parent);
        if (attachChildToParentResult.IsFailure) return attachChildToParentResult.Error.ToErrors();

        return createDepartmentChild;
    }

    private static UnitResult<Error> AttachChildToParent(Department child, Department parent)
    {
        parent.AttachChildToParent(child);
        return UnitResult.Success<Error>();
    }

    private UnitResult<Error> AttachChildToParent(Department child)
    {
        if (_childDepartments.Any(item => item.Identifier == child.Identifier))
            return Error.Conflict("child.already.exist", "Дочерний департамент уже привязан.");

        _childDepartments.Add(child);
        UpdatedAt = DateTimeOffset.UtcNow;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> AddLocation(LocationId locationId)
    {
        if (_departmentsLocations.Any(l => l.LocationId.Id == locationId.Id))
            return Error.Conflict("location.already.exist", "Location уже существует");

        var departmentLocation = new DepartmentLocation(Id, locationId);

        _departmentsLocations.Add(departmentLocation);
        UpdatedAt = DateTimeOffset.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> AddPosition(PositionId positionId)
    {
        if (_positions.Any(l => l.PositionId == positionId))
            return Error.Conflict("position.already.exist", "Position уже существует");

        var departmentPosition = new DepartmentPosition(Id, positionId);

        _positions.Add(departmentPosition);
        UpdatedAt = DateTimeOffset.UtcNow;

        return UnitResult.Success<Error>();
    }
}