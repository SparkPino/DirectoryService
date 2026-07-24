using DirectoryService.Application.Departments.Queries.GetDepartmentChildren;
using DirectoryService.Contracts.Department;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class GetDepartmentChildrenTest : DirectoryBaseTests
{
    public GetDepartmentChildrenTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetChildren_Should_Return_Only_Direct_Children()
    {
        var (alpha, beta, gamma, _, epsilon) = await SeedAlphaHierarchyAsync();
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<GetDepartmentChildrenQuery, List<DepartmentTreeNodeDto>>(async sut =>
            await sut.Handle(new GetDepartmentChildrenQuery(alpha), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Contains(result.Value, d => d.Id == beta);
        Assert.Contains(result.Value, d => d.Id == epsilon);
        Assert.DoesNotContain(result.Value, d => d.Id == gamma);
    }

    [Fact]
    public async Task GetChildren_NodeWithoutChildren_Should_Return_Empty_List()
    {
        var (_, _, gamma, _, _) = await SeedAlphaHierarchyAsync();
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<GetDepartmentChildrenQuery, List<DepartmentTreeNodeDto>>(async sut =>
            await sut.Handle(new GetDepartmentChildrenQuery(gamma), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetChildren_NonexistentDepartment_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<GetDepartmentChildrenQuery, List<DepartmentTreeNodeDto>>(async sut =>
            await sut.Handle(new GetDepartmentChildrenQuery(Guid.NewGuid()), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Code == "department.not.found");
    }
}
