using Core;
using DirectoryService.Application.Search;
using DirectoryService.Contracts.Search;
using DirectoryService.Presenters.Controllers;
using Microsoft.AspNetCore.Mvc;
using Shared.EndpointResult;

namespace DirectoryService.Presenters.GeneralSearch;

[ApiController]
[Route("/api/search/")]
public class SearchController : BaseApiController
{
    [HttpGet]
    public async Task<EndpointResult<List<SearchResultItemDto>>> Search(
        [FromQuery] GeneralSearchQuery query,
        [FromServices] IQueryHandler<GeneralSearchQuery, List<SearchResultItemDto>> queryHandler,
        CancellationToken cancellationToken) => await queryHandler.Handle(query, cancellationToken);
}