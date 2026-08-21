using Core.Validations;
using DirectoryService.Application.Departments.Failures;
using FluentValidation;

namespace DirectoryService.Application.Departments.Commands.MoveDepartment;

public class MoveDepartmentValidator : AbstractValidator<MoveDepartmentCommand>
{
    public MoveDepartmentValidator()
    {
        RuleFor(a => a.DepartmentId)
            .NotEmpty().WithError(DepartmentError.InvalidId);
    }
}