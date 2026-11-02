using System.Linq.Expressions;
using Core.Validations;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands;

public abstract class BulkDepartmentsValidator<TCommand> : AbstractValidator<TCommand>
{
    private const int MaxDepartments = 100;
    private const string InvalidField = "DepartmentIds";

    protected BulkDepartmentsValidator(Expression<Func<TCommand, List<Guid>>> departmentIds)
    {
        RuleFor(departmentIds)
            .Cascade(CascadeMode.Stop) // останавливает цепочку на первой ошибке, иначе Must упадёт на null
            .NotEmpty()
            .WithError(Error.Validation(
                "bulk.departments.empty",
                "Нужно выбрать хотя бы одно подразделение",
                InvalidField))
            .Must(ids => ids.Count <= MaxDepartments)
            .WithError(Error.Validation(
                "bulk.departments.too_many",
                $"Можно выбрать не больше {MaxDepartments} подразделений",
                InvalidField))
            .Must(ids => !ids.Contains(Guid.Empty))
            .WithError(Error.Validation(
                "bulk.departments.empty_id",
                "Идентификатор подразделения не может быть пустым",
                InvalidField))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithError(Error.Validation(
                "bulk.departments.duplicate_id",
                "Идентификаторы подразделений не должны повторяться",
                InvalidField));
    }
}
