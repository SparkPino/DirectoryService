using Core;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Locations.Commands.AddLocation;

public record AddLocationCommand(AddLocationDto AddLocationDto) : ICommand;