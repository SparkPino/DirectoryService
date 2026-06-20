using DirectoryService.Application.Positions.Failures;
using DirectoryService.Application.Validations;
using FluentValidation;

namespace DirectoryService.Application.Positions.RemovePosition;

public class RemovePositionValidator : AbstractValidator<RemovePositionCommand>
{
    public RemovePositionValidator()
    {
        RuleFor(p => p.PositionId)
            .NotEmpty().WithError(PositionErrors.InvalidId);
    }
}