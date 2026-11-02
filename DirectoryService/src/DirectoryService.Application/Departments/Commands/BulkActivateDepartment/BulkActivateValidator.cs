namespace DirectoryService.Application.Departments.Commands.BulkActivateDepartment;

public class BulkActivateValidator : BulkDepartmentsValidator<BulkActivateCommand>
{
    public BulkActivateValidator()
        : base(command => command.DepartmentIds)
    {
    }
}
