using System.Net;
using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Commands.BulkActivateDepartment;
using DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class BulkActivateDepartmentTest : DirectoryBaseTests
{
    private const string NotFoundCode = "bulk.activate.not.all.departments";
    private const string ParentInactiveCode = "department.activate.parent_inactive";
    private const string DeletedCode = "department.activate.deleted";

    private readonly HttpClient _client;

    public BulkActivateDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task BulkActivate_Should_Activate_Only_Selected_Departments()
    {
        // alpha → beta → (gamma, delta); alpha → epsilon
        var (_, _, gamma, delta, epsilon) = await SeedAlphaHierarchyAsync();
        await DeactivateInDbAsync(gamma, delta, epsilon);

        var result = await BulkActivateAsync(gamma, epsilon);

        Assert.True(result.IsSuccess);

        var departments = await GetDepartmentsAsync();

        Assert.True(departments[gamma].IsActive);
        Assert.True(departments[epsilon].IsActive);
        Assert.False(departments[delta].IsActive);
    }

    [Fact]
    public async Task BulkActivate_Root_Department_Should_Succeed()
    {
        var departmentId = await CreateDepartmentAsync("Solo", "solo");
        await DeactivateInDbAsync(departmentId);

        var result = await BulkActivateAsync(departmentId);

        Assert.True(result.IsSuccess);
        Assert.True((await GetDepartmentsAsync())[departmentId].IsActive);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BulkActivate_Parent_And_Child_Should_Work_In_Any_Order(bool childFirst)
    {
        var (alpha, beta, gamma, delta, epsilon) = await SeedAlphaHierarchyAsync();
        await DeactivateInDbAsync(alpha, beta, gamma, delta, epsilon);
        var ids = new[] { alpha, beta, gamma };

        var result = await BulkActivateAsync(childFirst ? ids.Reverse().ToArray() : ids);

        Assert.True(result.IsSuccess);

        var departments = await GetDepartmentsAsync();

        Assert.All(ids, id => Assert.True(departments[id].IsActive));
        Assert.False(departments[delta].IsActive);
        Assert.False(departments[epsilon].IsActive);
    }

    [Fact]
    public async Task BulkActivate_With_Inactive_Unselected_Parent_Should_Fail_And_Change_Nothing()
    {
        var (_, beta, gamma, _, epsilon) = await SeedAlphaHierarchyAsync();
        await DeactivateInDbAsync(beta, gamma, epsilon);

        // epsilon могла бы включиться сама, но у gamma выключен родитель
        var result = await BulkActivateAsync(gamma, epsilon);

        Assert.True(result.IsFailure);
        AssertConflictFor(result.Error, ParentInactiveCode, gamma);

        var departments = await GetDepartmentsAsync();

        Assert.False(departments[gamma].IsActive);
        Assert.False(departments[epsilon].IsActive);
    }

    [Fact]
    public async Task BulkActivate_Deleted_Department_Should_Fail_And_Change_Nothing()
    {
        var (_, _, gamma, _, epsilon) = await SeedAlphaHierarchyAsync();
        await SoftDeleteAsync(gamma);
        await DeactivateInDbAsync(epsilon);

        var result = await BulkActivateAsync(gamma, epsilon);

        Assert.True(result.IsFailure);
        AssertConflictFor(result.Error, DeletedCode, gamma);
        Assert.False((await GetDepartmentsAsync())[epsilon].IsActive);
    }

    [Fact]
    public async Task BulkActivate_Department_With_Deleted_Parent_Should_Return_Error_Not_Exception()
    {
        var (_, beta, gamma, _, _) = await SeedAlphaHierarchyAsync();

        // Архивация beta заодно архивирует gamma: родитель должен загрузиться с includingDeleted
        await SoftDeleteAsync(beta);

        var result = await BulkActivateAsync(gamma);

        Assert.True(result.IsFailure);
        AssertConflictFor(result.Error, DeletedCode, gamma);
    }

    [Fact]
    public async Task BulkActivate_Should_Collect_All_Errors()
    {
        var (_, beta, gamma, delta, _) = await SeedAlphaHierarchyAsync();
        await DeactivateInDbAsync(beta, gamma, delta);
        var missingId = Guid.NewGuid();

        var result = await BulkActivateAsync(gamma, delta, missingId);

        Assert.True(result.IsFailure);
        Assert.Equal(3, result.Error.Count());
        AssertConflictFor(result.Error, ParentInactiveCode, gamma);
        AssertConflictFor(result.Error, ParentInactiveCode, delta);
        AssertConflictFor(result.Error, NotFoundCode, missingId);
    }

    [Fact]
    public async Task BulkActivate_Already_Active_Department_Should_Succeed()
    {
        var (_, _, gamma, delta, _) = await SeedAlphaHierarchyAsync();
        await DeactivateInDbAsync(delta);

        var result = await BulkActivateAsync(gamma, delta);

        Assert.True(result.IsSuccess);

        var departments = await GetDepartmentsAsync();

        Assert.True(departments[gamma].IsActive);
        Assert.True(departments[delta].IsActive);
    }

    [Fact]
    public async Task Endpoint_Should_Return_200_And_Activate_Department()
    {
        var departmentId = await CreateDepartmentAsync("Solo", "solo");
        await DeactivateInDbAsync(departmentId);

        var response = await PostAsync(departmentId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True((await GetDepartmentsAsync())[departmentId].IsActive);
    }

    [Fact]
    public async Task Endpoint_With_Missing_Id_And_Domain_Conflict_Should_Return_409_And_Change_Nothing()
    {
        var (_, beta, gamma, _, epsilon) = await SeedAlphaHierarchyAsync();
        await DeactivateInDbAsync(beta, gamma, epsilon);

        var response = await PostAsync(gamma, epsilon, Guid.NewGuid());

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var departments = await GetDepartmentsAsync();

        Assert.False(departments[gamma].IsActive);
        Assert.False(departments[epsilon].IsActive);
    }

    private static void AssertConflictFor(Errors errors, string code, Guid departmentId) =>
        Assert.Contains(
            errors,
            e => e.Code == code && e.Type == ErrorType.CONFLICT && e.InvalidField == departmentId.ToString());

    private Task<HttpResponseMessage> PostAsync(params Guid[] departmentIds) =>
        _client.PostAsJsonAsync("/api/departments/bulk/activate", new BulkDepartmentsDto(departmentIds.ToList()));

    private Task<Result<Unit, Errors>> BulkActivateAsync(params Guid[] departmentIds) =>
        ExecuteHandler<BulkActivateCommand, Unit>(async sut =>
            await sut.Handle(new BulkActivateCommand(departmentIds.ToList()), CancellationToken.None));

    private Task<Result<Unit, Errors>> SoftDeleteAsync(Guid departmentId) =>
        ExecuteHandler<SoftDeleteDepartmentCommand, Unit>(async sut =>
            await sut.Handle(new SoftDeleteDepartmentCommand(new DepartmentId(departmentId)), CancellationToken.None));

    private Task DeactivateInDbAsync(params Guid[] departmentIds) =>
        ExecuteInDb(async db =>
        {
            var ids = departmentIds.Select(id => new DepartmentId(id)).ToList();
            var departments = await db.Departments
                .Where(d => ids.Contains(d.Id))
                .ToListAsync();

            foreach (var department in departments)
            {
                Assert.True(department.Deactivate([], DateTimeOffset.UtcNow).IsSuccess);
            }

            await db.SaveChangesAsync();
        });

    private Task<Dictionary<Guid, Department>> GetDepartmentsAsync() =>
        ExecuteInDb(async db => await db.Departments
            .IgnoreQueryFilters()
            .ToDictionaryAsync(d => d.Id.Id));
}
