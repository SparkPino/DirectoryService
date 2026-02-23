namespace DirectoryService.Domain;

public class DepartmentsLocation
{
    public required Guid Id { get; init; }

    public Guid DepartmentId { get; set; }

    public Guid LocationId { get; set; }
}