using Core.Validations;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Search;

public class GeneralSearchValidator : AbstractValidator<GeneralSearchQuery>
{
    public GeneralSearchValidator()
    {
        // Валидируем не само свойство Search, а вычисленное значение - Search после Trim,
        // чтобы "  a  " не проходило MinimumLength(2). Хендлер получает запрос без обрезки.
        // Из такого выражения FluentValidation не может вывести имя свойства (как из q => q.Search),
        // поэтому имя правила задаётся ниже через OverridePropertyName.
        RuleFor(q => q.Search == null ? null : q.Search.Trim())
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
                "Search"))

            // Задаёт имя всему правилу (PropertyName в ошибке и {PropertyName} в сообщениях).
            // Без него имени нет вообще, и FluentValidation бросит исключение при валидации.
            // Позиция в цепочке не важна, но вызвать можно только после первого валидатора.
            .OverridePropertyName("Search");

        RuleFor(q => q.Limit)
            .Must(limit => limit is null || (limit > 0 && limit < 100))
            .WithError(Error.Validation(
                "limit.out.of.range",
                "Limit должен быть от 1 до 99",
                "Limit"));
    }
}