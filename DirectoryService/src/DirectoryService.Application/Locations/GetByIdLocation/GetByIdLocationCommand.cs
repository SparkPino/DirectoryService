using DirectoryService.Domain.Locations.ValueObjects;
using ICommand = DirectoryService.Application.Abstraction.ICommand;

namespace DirectoryService.Application.Locations.GetByIdLocation;

public record GetByIdLocationCommand(LocationId LocationId) : ICommand;