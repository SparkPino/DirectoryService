using Core.Validations;
using DirectoryService.Application.Departments.Failures;
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