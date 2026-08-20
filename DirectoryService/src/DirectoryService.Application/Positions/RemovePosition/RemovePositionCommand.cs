using Core;

namespace DirectoryService.Application.Positions.RemovePosition;

public record RemovePositionCommand(Guid PositionId) : ICommand;