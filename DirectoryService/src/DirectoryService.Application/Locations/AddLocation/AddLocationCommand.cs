using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Locations.AddLocation;

public record AddLocationCommand(LocationDto LocationDto) : ICommand;