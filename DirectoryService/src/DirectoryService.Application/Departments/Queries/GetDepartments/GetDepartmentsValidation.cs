using Core.Validations;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Queries.GetDepartments;

public class GetDepartmentsValidation : AbstractValidator<GetDepartmentsQuery>
{
    private static readonly string[] AllowedSortBy = ["name", "created_at"];
    private static readonly SortDirection[] AllowedSortDirection = [SortDirection.DESC, SortDirection.ASC];

    public GetDepartmentsValidation()
    {
        When(a => a.Pagination is not null, () =>
        {
            RuleFor(a => a.Pagination.Page).Must(a => a > 0)
                .WithError(Error.Validation("page.invalid", "Page должно быть не меньше 1", "Page"));

            RuleFor(a => a.Pagination.PageSize).Must(a => a is > 0 && a <= 100)
                .WithError(Error.Validation("pageSize.invalid", "pageSize должен быть от 1 до 100", "PageSize"));
        });
        RuleFor(a => a.Search).Must(a => a is null || a.Length <= 150)
            .WithError(Error.Validation("search.invalid", "search не может быть длиннее 150 символов ",
                "Search"));

        RuleFor(a => a.SortBy).Must(a => a is null || AllowedSortBy.Contains(a))
            .WithError(Error.Validation(
                "sortBy.invalid",
                "sortBy может быть одним из: name, created_at",
                "SortBy"));

        RuleFor(a => a.SortDir).IsInEnum() //проверяет, что значение является одним из объявленных членов enum
            .WithError(Error.Validation(
                "sortDir.invalid",
                "sortDir может быть одним из: ASC, DESC ",
                "SortDir"));

        RuleFor(a => a.LocationIds).Must(ids => ids is null || ids.Count <= 100)
            .WithError(Error.Validation(
                "locationIds.invalid",
                "locationIds может содержать не больше 100 значений",
                "LocationIds"));


        RuleFor(a => a.ExcludeIds).Must(ids => ids is null || ids.Count <= 100)
            .WithError(Error.Validation(
                "excludeIds.invalid",
                "excludeIds может содержать не больше 100 значений ",
                "ExcludeIds"));

        RuleFor(a => a.ParentId).Must(a => a is null || a.Value != Guid.Empty)
            .WithError(Error.Validation(
                "parentId.invalid",
                "parentId не может быть пустым идентификатором",
                "ParentId"));
    }
}