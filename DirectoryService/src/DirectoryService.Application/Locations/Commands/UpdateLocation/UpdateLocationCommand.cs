using Core;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Locations.Commands.UpdateLocation;

public record UpdateLocationCommand(UpdateLocationDto UpdateLocationDto, Guid Id) : ICommand;