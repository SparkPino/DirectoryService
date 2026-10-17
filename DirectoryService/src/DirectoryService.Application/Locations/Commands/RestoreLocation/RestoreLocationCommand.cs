using Core;

namespace DirectoryService.Application.Locations.Commands.RestoreLocation;

public record RestoreLocationCommand(Guid LocationId) : ICommand;
