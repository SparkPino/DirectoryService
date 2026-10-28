using Core;
using Core.Validations;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Contracts.Search;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Search;

public class GeneralSearchHandler : IQueryHandler<GeneralSearchQuery, List<SearchResultItemDto>>
{
    private readonly ISearchReadRepository _searchReadRepository;
    private readonly IValidator<GeneralSearchQuery> _validator;
    private const int DefaultLimit = 10;

    public GeneralSearchHandler(ISearchReadRepository searchReadRepository, IValidator<GeneralSearchQuery> validator)
    {
        _searchReadRepository = searchReadRepository;
        _validator = validator;
    }

    public async Task<Result<List<SearchResultItemDto>, Errors>> Handle(GeneralSearchQuery query,
        CancellationToken cancellationToken)
    {
        var normalize = query with { Search = query.Search?.Trim() };

        var validationResult = await _validator.ValidateAsync(normalize, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        var result =
            await _searchReadRepository.GeneralSearch(normalize.Search!, normalize.Limit ?? DefaultLimit,
                cancellationToken);
        return result;
    }
}