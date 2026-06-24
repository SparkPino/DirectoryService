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

    private readonly List<DepartmentPosition> _departmentPositions = [];

    private readonly List<Department> _childDepartments = [];

    private Department(
        DepartmentId id,
        DepartmentName name,
        DepartmentIdentifier identifier,
        DepartmentPath path,
        short depth,
        IEnumerable<DepartmentLocation>? locations,
        IEnumerable<DepartmentPosition>? positions,
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
        _departmentPositions = positions.ToList();
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

    public IReadOnlyList<DepartmentLocation> DepartmentsLocations => _departmentsLocations.AsReadOnly();

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions.AsReadOnly();

    public static Result<Department, Errors> CreateDepartment(
        IEnumerable<DepartmentPosition> positions,
        DepartmentName name,
        DepartmentIdentifier identifier,
        Department? parent = null)
    {
        return parent is null
            ? Department.CreateRoot(positions, name, identifier)
            : Department.CreateChild(positions, name, identifier, parent);
    }

    private static Result<Department, Errors> CreateRoot(
        IEnumerable<DepartmentPosition> positions,
        DepartmentName name,
        DepartmentIdentifier identifier)
    {
        const int depth = 0;

        var pathResult = DepartmentPath.CreateParent(identifier);
        if (pathResult.IsFailure) return pathResult.Error;

        return Create(
            positions, [],
            name, identifier,
            pathResult.Value,
            depth);
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

    private Result<Department, Errors> CreateChild(
        IEnumerable<DepartmentPosition> positions,
        DepartmentName name,
        DepartmentIdentifier identifier)
    {
        var childPath = Path.CreateChildPath(identifier);
        if (childPath.IsFailure) return childPath.Error;

        short childDepth = (short)(Depth + 1);

        var createDepartmentChild = Create(
            positions,
            [],
            name,
            identifier,
            childPath.Value,
            childDepth,
            Id);

        var attachChildToParentResult = AttachChildToParent(createDepartmentChild);
        if (attachChildToParentResult.IsFailure) return attachChildToParentResult.Error.ToErrors();

        return createDepartmentChild;
    }

    private static Result<Department, Errors> CreateChild(
        IEnumerable<DepartmentPosition> positions,
        DepartmentName name,
        DepartmentIdentifier identifier,
        Department parent)
    {
        var childPath = parent.Path.CreateChildPath(identifier);
        if (childPath.IsFailure) return childPath.Error;
        short childDepth = (short)(parent.Depth + 1);

        var createDepartmentChild = Create(
            positions,
            [],
            name,
            identifier,
            childPath.Value,
            childDepth,
            parent.Id);

        var attachChildToParentResult = parent.AttachChildToParent(createDepartmentChild);
        if (attachChildToParentResult.IsFailure) return attachChildToParentResult.Error.ToErrors();

        return createDepartmentChild;
    }

    private UnitResult<Error> AttachChildToParent(Department child)
    {
        if (_childDepartments.Any(item => item.Identifier == child.Identifier))
            return Error.Conflict("child.already.exist", "Дочерний департамент уже привязан.");

        _childDepartments.Add(child);
        UpdatedAt = DateTimeOffset.UtcNow;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> DetachLocation(LocationId locationId)
    {
        if (!_departmentsLocations.Any(l => l.LocationId == locationId))
        {
            return Error.NotFound("location.not.foud", $"location с id:{locationId} не найдена");
        }

        var departmentLocation = _departmentsLocations.FirstOrDefault(a => a.LocationId == locationId);
        _departmentsLocations.Remove(departmentLocation!);
        UpdatedAt = DateTimeOffset.UtcNow;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> AddLocation(LocationId locationId)
    {
        var departmentLocation = new DepartmentLocation(Id, locationId);

        _departmentsLocations.Add(departmentLocation);
        UpdatedAt = DateTimeOffset.UtcNow;

        return UnitResult.Success<Error>();
    }


    public UnitResult<Error> AddLocations(IEnumerable<LocationId> locationId)
    {
        var duplicates = locationId
            .Where(g => _departmentsLocations
                .Any(l => l.LocationId.Id == g.Id)).ToList();
        if (duplicates.Count > 0)
        {
            return Error.Conflict(
                "location.already.exist",
                $"Locations с id:{string.Join(", ", duplicates.Select(d => d.Id))} уже существуют");
        }

        var departmentLocation = locationId.Select(l => new DepartmentLocation(Id, l));

        _departmentsLocations.AddRange(departmentLocation);
        UpdatedAt = DateTimeOffset.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> AddPosition(PositionId positionId)
    {
        var departmentPosition = new DepartmentPosition(Id, positionId);

        _departmentPositions.Add(departmentPosition);
        UpdatedAt = DateTimeOffset.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> DetachPosition(PositionId positionId)
    {
        if (!_departmentPositions.Any(p => p.PositionId == positionId))
        {
            return Error.NotFound("position.not.found", $"position с id:{positionId} не найдена");
        }

        var departmentPosition = _departmentPositions.FirstOrDefault(p => p.PositionId == positionId);
        _departmentPositions.Remove(departmentPosition!);
        UpdatedAt = DateTimeOffset.UtcNow;
        return UnitResult.Success<Error>();
    }

    public Result<short, Errors> Relocate(Department? newParent)
    {
        if (newParent is not null)
        {
            bool movesIntoSelfOrDescendant =
                newParent.Path.Path == Path.Path ||
                newParent.Path.Path.StartsWith($"{Path.Path}.", StringComparison.Ordinal);

            if (movesIntoSelfOrDescendant)
            {
                return Error.Conflict(
                    "department.move.invalid_target",
                    "Департамент не может быть перемещён в самого себя или в собственного потомка").ToErrors();
            }
        }

        var pathResult = newParent is null
            ? DepartmentPath.CreateParent(Identifier)
            : newParent.Path.CreateChildPath(Identifier);

        if (pathResult.IsFailure) return pathResult.Error;

        short oldDepth = Depth;
        short newDepth = newParent is null ? (short)0 : (short)(newParent.Depth + 1);

        Path = pathResult.Value;
        Depth = newDepth;
        ParentId = newParent?.Id;
        UpdatedAt = DateTimeOffset.UtcNow;

        return (short)(newDepth - oldDepth);
    }

    public UnitResult<Errors> UpdateDepartment(
        DepartmentName? name = null,
        DepartmentIdentifier? identifier = null)
    {
        if (name is null && identifier is null)
            return Error.Validation("update.department", "нужно указать хотябы одно поле").ToErrors();

        Name = name ?? Name;
        Identifier = identifier ?? Identifier;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Errors>();
    }
}