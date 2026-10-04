using Core;
using DirectoryService.Contracts.Positions;

namespace DirectoryService.Application.Positions.UpdatePosition;

public record UpdatePositionCommand(UpdatePositionDto UpdatePositionDto, Guid Id) : ICommand;