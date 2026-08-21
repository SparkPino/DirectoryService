namespace DirectoryService.Application.Abstraction.Repositories;

public interface ILocationReadRepository
{
    IQueryable<LocationRow> SearchLocations(string? search);
}