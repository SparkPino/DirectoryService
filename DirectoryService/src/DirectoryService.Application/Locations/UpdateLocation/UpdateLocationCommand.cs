using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Locations.UpdateLocation;

public record UpdateLocationCommand(UpdateLocationDto UpdateLocationDto, Guid Id) : ICommand<Guid>;