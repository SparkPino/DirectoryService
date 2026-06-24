using DirectoryService.Application.Departments.Failures;
using DirectoryService.Application.Validations;
using FluentValidation;

namespace DirectoryService.Application.Departments.Commands.RemoveDepartment;

public class RemoveDepartmentValidator : AbstractValidator<RemoveDepartmentCommand>
{
    public RemoveDepartmentValidator()
    {
        RuleFor(a => a.departmentId)
            .NotEmpty().WithError(DepartmentError.InvalidId);

    }
}