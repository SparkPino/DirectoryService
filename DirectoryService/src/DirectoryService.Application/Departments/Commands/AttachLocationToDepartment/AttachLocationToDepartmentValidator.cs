using Core.Validations;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Application.Locations.Failures;
using FluentValidation;

namespace DirectoryService.Application.Departments.Commands.AttachLocationToDepartment;

public class AttachLocationToDepartmentValidator : AbstractValidator<AttachLocationToDepartmentCommand>
{
    public AttachLocationToDepartmentValidator()
    {
        RuleFor(a => a.LocationId)
            .NotEmpty().WithError(LocationErrors.InvalidId);
        
        RuleFor(a => a.DepartmentId)
            .NotEmpty().WithError(DepartmentError.InvalidId);
    }
}