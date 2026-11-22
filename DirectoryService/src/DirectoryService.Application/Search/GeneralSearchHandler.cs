using Core;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Contracts.Search;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Search;

public class GeneralSearchHandler : IQueryHandler<GeneralSearchQuery, List<SearchResultItemDto>>
{
    private readonly ISearchReadRepository _searchReadRepository;
    private const int DefaultLimit = 10;

    public GeneralSearchHandler(ISearchReadRepository searchReadRepository)
    {
        _searchReadRepository = searchReadRepository;
    }

    public async Task<Result<List<SearchResultItemDto>, Errors>> Handle(GeneralSearchQuery query,
        CancellationToken cancellationToken)
    {
        // Валидатор (через декоратор) уже проверил обрезанную строку, здесь обрезаем то, что уходит в БД.
        var result =
            await _searchReadRepository.GeneralSearch(query.Search!.Trim(), query.Limit ?? DefaultLimit,
                cancellationToken);
        return result;
    }
}