using DirectoryService.Application.Departments.Failures;
using DirectoryService.Application.Positions.Failures;
using DirectoryService.Application.Validations;
using FluentValidation;

namespace DirectoryService.Application.Departments.Commands.AttachPositionToDepartment;

public class AttachPositionToDepartmentValidator : AbstractValidator<AttachPositionToDepartmentCommand>
{
    public AttachPositionToDepartmentValidator()
    {
        RuleFor(a => a.PositionId)
            .NotEmpty().WithError(PositionErrors.InvalidId);

        RuleFor(a => a.DepartmentId)
            .NotEmpty().WithError(DepartmentError.InvalidId);
    }
}