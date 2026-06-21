using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Positions;

namespace DirectoryService.Application.Positions.AddPosition;

public record AddPositionCommand(AddPositionDto AddPositionDto) : ICommand;