using Core.Validations;
using DirectoryService.Application.Positions.Failures;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Positions.UpdatePosition;

public class UpdatePositionValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithError(PositionErrors.InvalidId);

        RuleFor(p => p.UpdatePositionDto.Name)
            .MustBeValueObject(PositionName.Create)
            .When(p => p.UpdatePositionDto.Name != null);
    }
}