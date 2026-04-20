using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Locations.AddLocation;

public record AddLocationCommand(LocationDto LocationDto) : ICommand;