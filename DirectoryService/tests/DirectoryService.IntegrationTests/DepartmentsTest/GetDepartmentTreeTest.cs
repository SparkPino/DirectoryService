using DirectoryService.Application.Departments.Queries.GetDepartmentTree;
using DirectoryService.Contracts.Department;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class GetDepartmentTreeTest : DirectoryBaseTests
{
    public GetDepartmentTreeTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetTree_EmptyDatabase_Should_Return_Empty_List()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<GetDepartmentTreeQuery, List<DepartmentTreeNodeDto>>(async sut =>
            await sut.Handle(new GetDepartmentTreeQuery(null, null), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetTree_Should_Return_Only_Root_Nodes_With_HasChildren_Flag()
    {
        var (alpha, beta, _, _, epsilon) = await SeedAlphaHierarchyAsync();
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<GetDepartmentTreeQuery, List<DepartmentTreeNodeDto>>(async sut =>
            await sut.Handle(new GetDepartmentTreeQuery(null, null), cancellationToken));

        Assert.True(result.IsSuccess);
        var root = Assert.Single(result.Value);

        Assert.Equal(alpha, root.Id);
        Assert.True(root.HasChildren);
        Assert.Equal(2, root.ChildrenCount);
        Assert.DoesNotContain(result.Value, d => d.Id == beta || d.Id == epsilon);
    }
}