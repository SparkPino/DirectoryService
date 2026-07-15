using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Locations.ValueObjects;

namespace DirectoryService.Application.Locations.Commands.SoftDeleteLocation;

public record SoftDeleteLocationCommand(LocationId Id) : ICommand;
