using Core.Validations;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Departments.Commands.GetAllChildren;

public class GetAllChildrenDepartmentValidatior : AbstractValidator<GetAllChildrenQuery>
{
    public GetAllChildrenDepartmentValidatior()
    {
        RuleFor(a => a.rootIdentifier)
            .MustBeValueObject(DepartmentIdentifier.Create);
    }
}