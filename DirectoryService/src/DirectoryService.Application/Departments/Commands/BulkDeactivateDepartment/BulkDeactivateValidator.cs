namespace DirectoryService.Application.Departments.Commands.BulkDeactivateDepartment;

public class BulkDeactivateValidator : BulkDepartmentsValidator<BulkDeactivateCommand>
{
    public BulkDeactivateValidator()
        : base(command => command.DepartmentIds)
    {
    }
}
