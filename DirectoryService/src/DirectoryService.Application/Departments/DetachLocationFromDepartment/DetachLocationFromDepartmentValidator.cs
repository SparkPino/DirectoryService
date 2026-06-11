using DirectoryService.Application.Departments.Failures;
using DirectoryService.Application.Locations.Failures;
using DirectoryService.Application.Validations;
using FluentValidation;

namespace DirectoryService.Application.Departments.DetachLocationFromDepartment;

public class DetachLocationFromDepartmentValidator : AbstractValidator<DetachLocationFromDepartmentCommand>
{
    public DetachLocationFromDepartmentValidator()
    {
        RuleFor(a => a.LocationId)
            .NotEmpty().WithError(LocationErrors.InvalidId);

        RuleFor(a => a.DepartmentId)
            .NotEmpty().WithError(DepartmentError.InvalidId);
    }
}