using DirectoryService.Application.Departments.Commands.GetAllChildren;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests;

public class LtreeTest : DirectoryBaseTests
{
    public LtreeTest(DirectoryServiceWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAllChildren_WithNestedHierarchy_Should_Return_Full_Descendant_Tree()
    {
        //Arrange
        var cancellationToken = CancellationToken.None;
        // Alpha
        //  ├─ Beta
        //  │   ├─ Gamma
        //  │   └─ Delta
        //  └─ Epsilon
        var (alpha, beta, gamma, delta, epsilon) = await SeedAlphaHierarchyAsync();

        var query = new GetAllChildrenQuery("alpha");

        //Act
        var result = await ExecuteQueryHandler<GetAllChildrenQuery, GetAllDepartmentChildrenDto>(async handler =>
            await handler.Handle(query, cancellationToken));

        //Assert
        Assert.True(result.IsSuccess);
        var root = result.Value;

        Assert.Equal(alpha, root.Id);
        Assert.Null(root.ParentId);
        Assert.Equal(2, root.childrens?.Count);

        var betaObject = Assert.Single(root.childrens, c => c.Id == beta);
        Assert.Equal(root.Id, betaObject.ParentId);
        Assert.Contains(betaObject.childrens, c => c.Id == gamma);
        Assert.Contains(betaObject.childrens, c => c.Id == delta);

        var singleEpsilon = Assert.Single(root.childrens, c => c.Id == epsilon);
        Assert.Empty(singleEpsilon.childrens!);
    }

    [Fact]
    public async Task GetAllChildren_FromInternalNode_Should_Return_Only_Its_Own_Subtree()
    {
        //Arrange
        var (alpha, beta, gamma, delta, epsilon) = await SeedAlphaHierarchyAsync();
        var cancellationToken = CancellationToken.None;
        var query = new GetAllChildrenQuery("beta");
        //Act
        var result =
            await ExecuteQueryHandler<GetAllChildrenQuery, GetAllDepartmentChildrenDto>(async sut =>
                await sut.Handle(query, cancellationToken));

        //Assert
        Assert.True(result.IsSuccess);
        var root = result.Value;

        ExecuteInDb(async context =>
        {
            var level = await context.Departments.Where(a => a.Id == new DepartmentId(beta))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            Assert.Equal(GetLevelFromPath(level.Path.Path), root.Level);
        });

        Assert.Equal(beta, root.Id);
        Assert.Equal(alpha, root.ParentId);
        Assert.Contains(root.childrens, c => c.Id == gamma);
        Assert.Contains(root.childrens, c => c.Id == delta);
        Assert.DoesNotContain(root.childrens, c => c.Id == alpha);
    }


    private int GetLevelFromPath(string path)
    {
        string[] result = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
        return result.Length;
    }
}