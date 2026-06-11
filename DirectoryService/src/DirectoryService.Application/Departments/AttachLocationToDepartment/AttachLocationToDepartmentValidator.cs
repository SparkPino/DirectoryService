using DirectoryService.Application.Departments.Failures;
using DirectoryService.Application.Locations.Failures;
using DirectoryService.Application.Validations;
using FluentValidation;

namespace DirectoryService.Application.Departments.AttachLocationToDepartment;

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