using Core.Validations;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Queries.SearchDepartmentTree;

public class SearchDepartmentTreeValidator : AbstractValidator<SearchDepartmentTreeQuery>
{
    public SearchDepartmentTreeValidator()
    {
        RuleFor(x => x.Q)
            .NotEmpty()
            .WithError(Error.Validation("q.empty", "Поисковый запрос не может быть пустым"))
            .MinimumLength(2)
            .WithError(Error.Validation("q.too_short", "Минимальная длина поискового запроса — 2 символа"));
    }
}
