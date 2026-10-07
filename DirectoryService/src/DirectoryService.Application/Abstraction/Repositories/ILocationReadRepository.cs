namespace DirectoryService.Application.Abstraction.Repositories;

public interface ILocationReadRepository
{
    IQueryable<LocationRow> SearchLocations(List<Guid>? departmentIds,string? search);
}