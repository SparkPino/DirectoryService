using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Locations.RemoveLocation;

public record RemoveLocationCommand(Guid LocationId) : ICommand;