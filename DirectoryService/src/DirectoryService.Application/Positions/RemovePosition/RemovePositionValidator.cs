using Core.Validations;
using DirectoryService.Application.Positions.Failures;
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