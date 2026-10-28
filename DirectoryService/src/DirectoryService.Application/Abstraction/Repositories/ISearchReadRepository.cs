using DirectoryService.Contracts.Search;

namespace DirectoryService.Application.Abstraction.Repositories;

public interface ISearchReadRepository
{
    public Task<List<SearchResultItemDto>> GeneralSearch(string query, int limit, CancellationToken cancellationToken);
}