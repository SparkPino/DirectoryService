using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Positions.RemovePosition;

public record RemovePositionCommand(Guid PositionId) : ICommand;