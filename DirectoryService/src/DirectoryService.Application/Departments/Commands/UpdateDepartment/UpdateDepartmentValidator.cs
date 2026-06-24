using DirectoryService.Application.Validations;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(a => a.DepartmentDto.Name)
            .MustBeValueObject(DepartmentName.Create!)
            .When(a => a.DepartmentDto.Name != null);

        RuleFor(a => a.DepartmentDto.Identifier)
            .MustBeValueObject(DepartmentIdentifier.Create!)
            .When(a => a.DepartmentDto.Identifier != null);
        ;
    }
}