using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Commands.RestoreDepartment;
using DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class RestoreDepartmentTest : DirectoryBaseTests
{
    public RestoreDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Restore_Should_Restore_Department_With_Its_Subtree()
    {
        // alpha → beta → (gamma, delta); alpha → epsilon
        var (_, beta, gamma, delta, _) = await SeedAlphaHierarchyAsync();
        await SoftDeleteAsync(beta);

        var result = await RestoreAsync(beta);

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var departments = await db.Departments
                .IgnoreQueryFilters()
                .ToDictionaryAsync(d => d.Id.Id);

            foreach (var id in new[] { beta, gamma, delta })
            {
                Assert.True(departments[id].IsActive);
                Assert.Null(departments[id].DeletedAt);
            }
        });
    }

    [Fact]
    public async Task Restore_Should_Not_Restore_Separately_Deleted_Descendant()
    {
        var (_, beta, gamma, delta, _) = await SeedAlphaHierarchyAsync();
        await SoftDeleteAsync(gamma);
        await SoftDeleteAsync(beta);

        var result = await RestoreAsync(beta);

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var departments = await db.Departments
                .IgnoreQueryFilters()
                .ToDictionaryAsync(d => d.Id.Id);

            Assert.True(departments[beta].IsActive);
            Assert.True(departments[delta].IsActive);
            Assert.False(departments[gamma].IsActive);
        });
    }

    [Fact]
    public async Task Restore_When_Parent_Is_Deleted_Should_Fail()
    {
        var (alpha, beta, _, _, _) = await SeedAlphaHierarchyAsync();
        await SoftDeleteAsync(alpha);

        var result = await RestoreAsync(beta);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Code == "root.department.deleted");
    }

    [Fact]
    public async Task Restore_When_Path_Is_Taken_Should_Fail()
    {
        var oldId = await CreateDepartmentAsync("Solo", "solo");
        await SoftDeleteAsync(oldId);
        await CreateDepartmentAsync("Solo new", "solo");

        var result = await RestoreAsync(oldId);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Code == "department.path.taken");
    }

    [Fact]
    public async Task Restore_Nonexistent_Department_Should_Fail()
    {
        var result = await RestoreAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Code == "department.not.found");
    }

    private Task<Result<Unit, Errors>> SoftDeleteAsync(Guid departmentId) =>
        ExecuteHandler<SoftDeleteDepartmentCommand, Unit>(async sut =>
            await sut.Handle(new SoftDeleteDepartmentCommand(new DepartmentId(departmentId)), CancellationToken.None));

    private Task<Result<Guid, Errors>> RestoreAsync(Guid departmentId) =>
        ExecuteHandler<RestoreDepartmentCommand, Guid>(async sut =>
            await sut.Handle(new RestoreDepartmentCommand(departmentId), CancellationToken.None));
}
