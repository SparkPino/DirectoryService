using DirectoryService.Application.Validations;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetDirectChildrenDepartment;

public class GetDirectChildrenDepartmentValidator : AbstractValidator<GetDirectChildrenDepartmentQuery>
{
    public GetDirectChildrenDepartmentValidator()
    {
        RuleFor(a => a.ParentId).NotNull().NotEmpty()
            .WithError(Error.Validation(null, message: "Не может быть пустым или null"));
    }
}