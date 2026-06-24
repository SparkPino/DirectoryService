using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Locations.ValueObjects;

namespace DirectoryService.Application.Locations.Queries.GetByIdLocation;

public record GetByIdLocationQuery(LocationId LocationId) : IQuery;