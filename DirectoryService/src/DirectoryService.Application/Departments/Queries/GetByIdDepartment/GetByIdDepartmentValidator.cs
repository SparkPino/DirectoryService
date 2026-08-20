using Core.Validations;
using DirectoryService.Application.Departments.Failures;
using FluentValidation;

namespace DirectoryService.Application.Departments.Queries;

public class GetByIdDepartmentValidator : AbstractValidator<GetByIdDepartmentQuery>
{
    public GetByIdDepartmentValidator()
    {
        RuleFor(a => a.id)
            .NotNull()
            .NotEmpty()
            .WithError(DepartmentError.InvalidId);
    }
}