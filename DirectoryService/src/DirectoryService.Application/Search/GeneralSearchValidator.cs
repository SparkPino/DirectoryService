using Core.Validations;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Search;

public class GeneralSearchValidator : AbstractValidator<GeneralSearchQuery>
{
    public GeneralSearchValidator()
    {
        RuleFor(q => q.Search)
            .NotEmpty()
            .WithError(Error.Validation(
                "general_search.empty",
                "Поисковый запрос не может быть пустым",
                "Search"))
            .MinimumLength(2)
            .WithError(Error.Validation(
                "general_search.too_short",
                "Введите больше 1 символа",
                "Search"))
            .MaximumLength(100).WithError(Error.Validation(
                "general_search.too_long",
                "Максимальное количество символов 100",
                "Search"));

        RuleFor(q => q.Limit)
            .Must(limit => limit is null || (limit > 0 && limit < 100))
            .WithError(Error.Validation(
                "limit.out.of.range",
                "Limit должен быть от 1 до 99",
                "Limit"));
    }
}