using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;

namespace DirectoryService.Application.Locations.GetByIdLocation;

public record GetByIdLocationQuery(LocationId LocationId) : IQuery<Location>;