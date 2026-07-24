using DirectoryService.Application.Departments.Queries.GetDepartmentAncestors;
using DirectoryService.Contracts.Department;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class GetDepartmentAncestorsTest : DirectoryBaseTests
{
    public GetDepartmentAncestorsTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetAncestors_Root_Should_Return_Empty_List()
    {
        var (alpha, _, _, _, _) = await SeedAlphaHierarchyAsync();
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<GetDepartmentAncestorsQuery, List<DepartmentTreeNodeDto>>(async sut =>
            await sut.Handle(new GetDepartmentAncestorsQuery(alpha), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAncestors_DeepNode_Should_Return_Ancestors_From_Root_To_Parent()
    {
        var (alpha, beta, gamma, _, _) = await SeedAlphaHierarchyAsync();
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<GetDepartmentAncestorsQuery, List<DepartmentTreeNodeDto>>(async sut =>
            await sut.Handle(new GetDepartmentAncestorsQuery(gamma), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal(alpha, result.Value[0].Id);
        Assert.Equal(beta, result.Value[1].Id);
    }

    [Fact]
    public async Task GetAncestors_NonexistentDepartment_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<GetDepartmentAncestorsQuery, List<DepartmentTreeNodeDto>>(async sut =>
            await sut.Handle(new GetDepartmentAncestorsQuery(Guid.NewGuid()), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Code == "department.not.found");
    }
}
