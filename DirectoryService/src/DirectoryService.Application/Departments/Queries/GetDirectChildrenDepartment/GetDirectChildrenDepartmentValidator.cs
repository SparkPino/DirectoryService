using DirectoryService.Application.Validations;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetDirectChildrenDepartment;

public class GetDirectChildrenDepartmentValidator : AbstractValidator<GetDirectChildrenDepartmentQuery>
{
    public GetDirectChildrenDepartmentValidator()
    {
        RuleFor(a => a.ParentId)
            .NotEqual(Guid.Empty)
            .When(a => a.ParentId.HasValue)
            .WithError(Error.Validation("parent_id.empty", "ParentId не может быть Guid.Empty"));
    }
}