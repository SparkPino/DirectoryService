using Core.Validations;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Application.Positions.Failures;
using FluentValidation;

namespace DirectoryService.Application.Departments.Commands.DetachPositionFromDepartment;

public class DetachPositionFromDepartmentValidator : AbstractValidator<DetachPositionFromDepartmentCommand>
{
    public DetachPositionFromDepartmentValidator()
    {
        RuleFor(a => a.PositionId)
            .NotEmpty().WithError(PositionErrors.InvalidId);

        RuleFor(a => a.DepartmentId)
            .NotEmpty().WithError(DepartmentError.InvalidId);
    }
}