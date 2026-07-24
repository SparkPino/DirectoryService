namespace DirectoryService.Contracts.Department;

public class DepartmentSearchResultDto
{
    public DepartmentTreeNodeDto Node { get; set; } = null!;

    public List<DepartmentTreeNodeDto> Ancestors { get; set; } = [];
}
