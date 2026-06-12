using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Locations.ValueObjects;
using ICommand = DirectoryService.Application.Abstraction.ICommand;

namespace DirectoryService.Application.Locations.GetByIdLocation;

public record GetByIdLocationQuery(LocationId LocationId) : IQuery;