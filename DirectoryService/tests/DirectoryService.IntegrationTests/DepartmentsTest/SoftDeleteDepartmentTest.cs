using DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;
using DirectoryService.Application.Departments.Queries;
using DirectoryService.Application.Departments.Queries.GetDepartments;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class SoftDeleteDepartmentTest : DirectoryBaseTests
{
    public SoftDeleteDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task SoftDelete_Department_Should_Keep_Row_And_Mark_DeletedAt()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var handlerResult = await SoftDeleteAsync(departmentId);

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var department = await db.Departments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(departmentId), cancellationToken);

            Assert.NotNull(department);
            Assert.False(department!.IsActive);
            Assert.NotNull(department.DeletedAt);
        });
    }

    [Fact]
    public async Task SoftDelete_Department_Should_Be_Hidden_From_Default_Query()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        await SoftDeleteAsync(departmentId);

        await ExecuteInDb(async db =>
        {
            var department = await db.Departments
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(departmentId), cancellationToken);

            Assert.Null(department);
        });
    }

    [Fact]
    public async Task SoftDelete_Department_Should_Be_Hidden_From_GetById()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        await SoftDeleteAsync(departmentId);

        var result = await ExecuteQueryHandler<GetByIdDepartmentQuery, DepartmentDto>(async sut =>
            await sut.Handle(new GetByIdDepartmentQuery(departmentId), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Code == "department.not.found");
    }

    [Fact]
    public async Task SoftDelete_Department_Should_Be_Hidden_From_List()
    {
        var departmentId = await CreateDepartmentAsync("UniqueSoftDeleteName", "UniqueSoftDeleteIdent");
        var cancellationToken = CancellationToken.None;

        await SoftDeleteAsync(departmentId);

        var result = await ExecuteQueryHandler<GetDepartmentsQuery, PagedResult<GetDepartmentDto>>(async sut =>
            await sut.Handle(new GetDepartmentsQuery { Search = "UniqueSoftDeleteName", IsActive = true }, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(result.Value.Items, d => d.DepartmentId == departmentId);
    }

    [Fact]
    public async Task SoftDelete_Nonexistent_Department_Should_Fail()
    {
        var handlerResult = await SoftDeleteAsync(Guid.NewGuid());

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error, e => e.Code == "department.not.found");
    }

    [Fact]
    public async Task SoftDelete_Should_Cascade_To_Descendants_With_Same_DeletedAt()
    {
        // alpha → beta → (gamma, delta); alpha → epsilon
        var (alpha, beta, gamma, delta, epsilon) = await SeedAlphaHierarchyAsync();

        var handlerResult = await SoftDeleteAsync(beta);

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var departments = await db.Departments
                .IgnoreQueryFilters()
                .ToDictionaryAsync(d => d.Id.Id);

            // Удалено всё поддерево beta с одним штампом
            Assert.False(departments[beta].IsActive);
            Assert.False(departments[gamma].IsActive);
            Assert.False(departments[delta].IsActive);
            Assert.Equal(departments[beta].DeletedAt, departments[gamma].DeletedAt);
            Assert.Equal(departments[beta].DeletedAt, departments[delta].DeletedAt);

            // Родитель и соседняя ветка не затронуты
            Assert.True(departments[alpha].IsActive);
            Assert.True(departments[epsilon].IsActive);
            Assert.Null(departments[alpha].DeletedAt);
            Assert.Null(departments[epsilon].DeletedAt);
        });
    }

    [Fact]
    public async Task SoftDelete_Should_Not_Overwrite_DeletedAt_Of_Previously_Deleted_Descendant()
    {
        var (_, beta, gamma, _, _) = await SeedAlphaHierarchyAsync();

        await SoftDeleteAsync(gamma);
        var gammaDeletedAt = await GetDeletedAtAsync(gamma);

        await SoftDeleteAsync(beta);

        Assert.Equal(gammaDeletedAt, await GetDeletedAtAsync(gamma));
        Assert.NotEqual(gammaDeletedAt, await GetDeletedAtAsync(beta));
    }

    private Task<CSharpFunctionalExtensions.Result<Unit, Errors>> SoftDeleteAsync(Guid departmentId) =>
        ExecuteHandler<SoftDeleteDepartmentCommand, Unit>(async sut =>
            await sut.Handle(new SoftDeleteDepartmentCommand(new DepartmentId(departmentId)), CancellationToken.None));

    private Task<DateTimeOffset?> GetDeletedAtAsync(Guid departmentId) =>
        ExecuteInDb(async db => await db.Departments
            .IgnoreQueryFilters()
            .Where(d => d.Id == new DepartmentId(departmentId))
            .Select(d => d.DeletedAt)
            .FirstAsync());
}
