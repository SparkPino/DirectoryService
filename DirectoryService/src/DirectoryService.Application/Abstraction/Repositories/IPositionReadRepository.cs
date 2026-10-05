namespace DirectoryService.Application.Abstraction.Repositories;

public interface IPositionReadRepository
{
    IQueryable<PositionRow> SearchPositions(Guid? departmentId,string? search);
}
