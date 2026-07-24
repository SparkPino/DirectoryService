namespace DirectoryService.Contracts.Department;

public class GetAllDepartmentChildrenDto
{
    public Guid Id { get; init; }

    public Guid? ParentId { get; init; }

    public string Name { get; init; }

    public string Identifier { get; init; }

    public string Path { get; init; }

    public int Level { get; init; }

    public List<GetAllDepartmentChildrenDto>? childrens { get; init; } = [];
}