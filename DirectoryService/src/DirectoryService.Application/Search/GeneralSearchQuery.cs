using Core;

namespace DirectoryService.Application.Search;

public sealed record GeneralSearchQuery(string? Search, int? Limit) : IQuery;