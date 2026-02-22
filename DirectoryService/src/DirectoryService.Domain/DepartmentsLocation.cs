namespace DirectoryService.Domain;

public class DepartmentsLocation
{
    public Guid Id { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid LocationId { get; set; }
}