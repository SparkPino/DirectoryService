using DirectoryService.Application.Validations;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Positions.AddPosition;

public class AddPositionValidator : AbstractValidator<AddPositionCommand>
{
    public AddPositionValidator()
    {
        RuleFor(p => p.AddPositionDto.Name)
            .MustBeValueObject(PositionName.Create);
    }
}