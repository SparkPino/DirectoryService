namespace DirectoryService.Application.Departments.Commands.BulkRestoreDepartment;

public class BulkRestoreValidator : BulkDepartmentsValidator<BulkRestoreCommand>
{
    public BulkRestoreValidator()
        : base(command => command.DepartmentIds)
    {
    }
}
