using DirectoryService.Application.Departments.Failures;
using DirectoryService.Application.Validations;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Commands.UpdateDepartmentParent;

public class UpdateDepartmentParentValidation : AbstractValidator<UpdateDepartmentParentCommand>
{
    public UpdateDepartmentParentValidation()
    {
        RuleFor(ud => ud.Id)
            .NotEmpty().WithError(DepartmentError.InvalidId);


        RuleFor(ud => ud.ParentId!.Id)
            .Must(id => id != Guid.Empty) // id is null избыточна, id != Guid.Empty пропускает null.
            .WithError(DepartmentError.InvalidId);
    }
}