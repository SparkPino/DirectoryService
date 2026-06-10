using DirectoryService.Application.Validations;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Departments.AddDepartment;

public class AddDepartmentValidator : AbstractValidator<AddDepartmentCommand>
{
    public AddDepartmentValidator()
    {
        RuleFor(a => a.DepartmentDto.Name)
            .MustBeValueObject(DepartmentName.Create);


        RuleFor(a => a.DepartmentDto.Identifier)
            .MustBeValueObject(DepartmentIdentifier.Create);
    }
}