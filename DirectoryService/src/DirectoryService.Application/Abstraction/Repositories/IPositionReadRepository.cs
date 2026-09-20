namespace DirectoryService.Application.Abstraction.Repositories;

public interface IPositionReadRepository
{
    IQueryable<PositionRow> SearchPositions(string? search);
}
