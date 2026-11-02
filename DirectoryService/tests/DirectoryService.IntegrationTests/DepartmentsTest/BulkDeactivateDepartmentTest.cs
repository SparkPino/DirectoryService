using System.Net;
using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Commands.BulkDeactivateDepartment;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class BulkDeactivateDepartmentTest : DirectoryBaseTests
{
    private const string NotFoundCode = "bulk.deactivate.not.all.exist";
    private const string HasActiveChildrenCode = "department.deactivate.has_active_children";

    private readonly HttpClient _client;

    public BulkDeactivateDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task BulkDeactivate_Should_Deactivate_Only_Selected_Departments()
    {
        // alpha → beta → (gamma, delta); alpha → epsilon
        var (alpha, beta, gamma, delta, epsilon) = await SeedAlphaHierarchyAsync();

        var result = await BulkDeactivateAsync(gamma, delta);

        Assert.True(result.IsSuccess);

        var departments = await GetDepartmentsAsync();

        Assert.False(departments[gamma].IsActive);
        Assert.False(departments[delta].IsActive);
        Assert.True(departments[alpha].IsActive);
        Assert.True(departments[beta].IsActive);
        Assert.True(departments[epsilon].IsActive);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BulkDeactivate_Whole_Tree_Should_Work_In_Any_Order(bool rootFirst)
    {
        var (alpha, beta, gamma, delta, epsilon) = await SeedAlphaHierarchyAsync();
        var ids = new[] { gamma, delta, beta, epsilon, alpha };

        var result = await BulkDeactivateAsync(rootFirst ? ids.Reverse().ToArray() : ids);

        Assert.True(result.IsSuccess);

        var departments = await GetDepartmentsAsync();

        Assert.All(ids, id => Assert.False(departments[id].IsActive));
    }

    [Fact]
    public async Task BulkDeactivate_Parent_With_Active_Unselected_Child_Should_Fail_And_Change_Nothing()
    {
        var (alpha, beta, gamma, delta, epsilon) = await SeedAlphaHierarchyAsync();

        // gamma могла бы выключиться сама, но у beta остаётся активная delta
        var result = await BulkDeactivateAsync(beta, gamma);

        Assert.True(result.IsFailure);
        AssertConflictFor(result.Error, HasActiveChildrenCode, beta);

        var departments = await GetDepartmentsAsync();

        Assert.All(
            new[] { alpha, beta, gamma, delta, epsilon },
            id => Assert.True(departments[id].IsActive));
    }

    [Fact]
    public async Task BulkDeactivate_Should_Collect_All_Errors()
    {
        var (alpha, beta, _, _, _) = await SeedAlphaHierarchyAsync();
        var missingId = Guid.NewGuid();

        var result = await BulkDeactivateAsync(alpha, beta, missingId);

        Assert.True(result.IsFailure);
        Assert.Equal(3, result.Error.Count());
        AssertConflictFor(result.Error, HasActiveChildrenCode, alpha);
        AssertConflictFor(result.Error, HasActiveChildrenCode, beta);
        AssertConflictFor(result.Error, NotFoundCode, missingId);
    }

    [Fact]
    public async Task BulkDeactivate_Already_Inactive_Department_Should_Succeed()
    {
        var (_, _, gamma, delta, _) = await SeedAlphaHierarchyAsync();
        await DeactivateInDbAsync(gamma);

        var result = await BulkDeactivateAsync(gamma, delta);

        Assert.True(result.IsSuccess);

        var departments = await GetDepartmentsAsync();

        Assert.False(departments[gamma].IsActive);
        Assert.False(departments[delta].IsActive);
    }

    [Fact]
    public async Task Endpoint_Should_Return_200_And_Deactivate_Department()
    {
        var departmentId = await CreateDepartmentAsync("Solo", "solo");

        var response = await PostAsync(departmentId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False((await GetDepartmentsAsync())[departmentId].IsActive);
    }

    [Fact]
    public async Task Endpoint_With_Missing_Id_And_Domain_Conflict_Should_Return_409_And_Change_Nothing()
    {
        var (alpha, _, gamma, _, _) = await SeedAlphaHierarchyAsync();

        var response = await PostAsync(alpha, gamma, Guid.NewGuid());

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var departments = await GetDepartmentsAsync();

        Assert.True(departments[alpha].IsActive);
        Assert.True(departments[gamma].IsActive);
    }

    private static void AssertConflictFor(Errors errors, string code, Guid departmentId) =>
        Assert.Contains(
            errors,
            e => e.Code == code && e.Type == ErrorType.CONFLICT && e.InvalidField == departmentId.ToString());

    private Task<HttpResponseMessage> PostAsync(params Guid[] departmentIds) =>
        _client.PostAsJsonAsync("/api/departments/bulk/deactivate", new BulkDepartmentsDto(departmentIds.ToList()));

    private Task<Result<Unit, Errors>> BulkDeactivateAsync(params Guid[] departmentIds) =>
        ExecuteHandler<BulkDeactivateCommand, Unit>(async sut =>
            await sut.Handle(new BulkDeactivateCommand(departmentIds.ToList()), CancellationToken.None));

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
