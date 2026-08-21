using Core;
using DirectoryService.Domain.Positions.ValueObjects;

namespace DirectoryService.Application.Positions.SoftDeletePosition;

public record SoftDeletePositionCommand(PositionId Id) : ICommand;
