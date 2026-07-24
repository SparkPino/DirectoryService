using DirectoryService.Application.Departments.Queries.SearchDepartmentTree;
using DirectoryService.Contracts.Department;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class SearchDepartmentTreeTest : DirectoryBaseTests
{
    public SearchDepartmentTreeTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Search_NoMatches_Should_Return_Empty_List()
    {
        await SeedAlphaHierarchyAsync();
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<SearchDepartmentTreeQuery, List<DepartmentSearchResultDto>>(async sut =>
            await sut.Handle(new SearchDepartmentTreeQuery("zzznotfound"), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Search_Should_Return_Match_With_Ancestors()
    {
        var (alpha, beta, gamma, _, _) = await SeedAlphaHierarchyAsync();
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<SearchDepartmentTreeQuery, List<DepartmentSearchResultDto>>(async sut =>
            await sut.Handle(new SearchDepartmentTreeQuery("gamma"), cancellationToken));

        Assert.True(result.IsSuccess);
        var match = Assert.Single(result.Value);

        Assert.Equal(gamma, match.Node.Id);
        Assert.Equal(2, match.Ancestors.Count);
        Assert.Contains(match.Ancestors, a => a.Id == alpha);
        Assert.Contains(match.Ancestors, a => a.Id == beta);
    }

    [Fact]
    public async Task Search_TooShortQuery_Should_Fail_Validation()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<SearchDepartmentTreeQuery, List<DepartmentSearchResultDto>>(async sut =>
            await sut.Handle(new SearchDepartmentTreeQuery("a"), cancellationToken));

        Assert.True(result.IsFailure);
    }
}
