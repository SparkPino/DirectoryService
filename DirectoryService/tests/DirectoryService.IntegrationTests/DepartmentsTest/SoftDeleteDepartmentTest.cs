using DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;
using DirectoryService.Application.Departments.Queries;
using DirectoryService.Application.Departments.Queries.GetDepartments;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;

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

        var command = new SoftDeleteDepartmentCommand(new DepartmentId(departmentId));
        var handlerResult = await ExecuteHandler<SoftDeleteDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var department = await db.Departments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(departmentId), cancellationToken);

            Assert.NotNull(department);
            Assert.False(department!.IsActive);
            Assert.NotEqual(default, department.DeletedAt);
        });
    }

    [Fact]
    public async Task SoftDelete_Department_Should_Be_Hidden_From_Default_Query()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        await ExecuteHandler<SoftDeleteDepartmentCommand, Guid>(async sut =>
            await sut.Handle(new SoftDeleteDepartmentCommand(new DepartmentId(departmentId)), cancellationToken));

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

        await ExecuteHandler<SoftDeleteDepartmentCommand, Guid>(async sut =>
            await sut.Handle(new SoftDeleteDepartmentCommand(new DepartmentId(departmentId)), cancellationToken));

        var result = await ExecuteQueryHandler<GetByIdDepartmentQuery, DepartmentDto>(async sut =>
            await sut.Handle(new GetByIdDepartmentQuery(departmentId), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Code == "department.not_found");
    }

    [Fact]
    public async Task SoftDelete_Department_Should_Be_Hidden_From_List()
    {
        var departmentId = await CreateDepartmentAsync("UniqueSoftDeleteName", "UniqueSoftDeleteIdent");
        var cancellationToken = CancellationToken.None;

        await ExecuteHandler<SoftDeleteDepartmentCommand, Guid>(async sut =>
            await sut.Handle(new SoftDeleteDepartmentCommand(new DepartmentId(departmentId)), cancellationToken));

        var result = await ExecuteQueryHandler<GetDepartmentsQuery, PagedResult<GetDepartmentDto>>(async sut =>
            await sut.Handle(new GetDepartmentsQuery { Search = "UniqueSoftDeleteName" }, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(result.Value.Items, d => d.DepartmentId == departmentId);
    }

    [Fact]
    public async Task SoftDelete_Nonexistent_Department_Should_Fail()
    {
        var cancellationToken = CancellationToken.None;

        var command = new SoftDeleteDepartmentCommand(new DepartmentId(Guid.NewGuid()));
        var handlerResult = await ExecuteHandler<SoftDeleteDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }
}
