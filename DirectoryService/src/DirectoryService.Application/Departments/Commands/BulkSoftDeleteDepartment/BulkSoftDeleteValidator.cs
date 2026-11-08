namespace DirectoryService.Application.Departments.Commands.BulkSoftDeleteDepartment;

public class BulkSoftDeleteValidator : BulkDepartmentsValidator<BulkSoftDeleteCommand>
{
    public BulkSoftDeleteValidator()
        : base(command => command.DepartmentIds)
    {
    }
}
