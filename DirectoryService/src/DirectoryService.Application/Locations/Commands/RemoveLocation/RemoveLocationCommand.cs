using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Locations.Commands.RemoveLocation;

public record RemoveLocationCommand(Guid LocationId) : ICommand;