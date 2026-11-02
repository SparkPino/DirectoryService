using System.Net;
using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Commands.BulkSoftDeleteDepartment;
using DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class BulkSoftDeleteDepartmentTest : DirectoryBaseTests
{
    private const string NotFoundCode = "bulk.soft_delete.not.all.exist";

    private readonly HttpClient _client;

    public BulkSoftDeleteDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task BulkSoftDelete_Should_Archive_Selected_With_Their_Subtrees()
    {
        // alpha → beta → (gamma, delta); alpha → epsilon
        var (alpha, beta, gamma, delta, epsilon) = await SeedAlphaHierarchyAsync();

        var result = await BulkSoftDeleteAsync(beta, epsilon);

        Assert.True(result.IsSuccess);

        var departments = await GetDepartmentsAsync();

        foreach (var id in new[] { beta, gamma, delta, epsilon })
        {
            Assert.NotNull(departments[id].DeletedAt);
            Assert.False(departments[id].IsActive);
        }

        Assert.Null(departments[alpha].DeletedAt);
        Assert.True(departments[alpha].IsActive);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BulkSoftDelete_Parent_And_Child_Should_Work_In_Any_Order(bool childFirst)
    {
        var (_, beta, gamma, delta, _) = await SeedAlphaHierarchyAsync();

        var result = childFirst
            ? await BulkSoftDeleteAsync(gamma, beta)
            : await BulkSoftDeleteAsync(beta, gamma);

        Assert.True(result.IsSuccess);

        var departments = await GetDepartmentsAsync();

        Assert.NotNull(departments[beta].DeletedAt);
        Assert.Equal(departments[beta].DeletedAt, departments[gamma].DeletedAt);
        Assert.Equal(departments[beta].DeletedAt, departments[delta].DeletedAt);
    }

    [Fact]
    public async Task BulkSoftDelete_Should_Not_Overwrite_DeletedAt_Of_Already_Deleted_Department()
    {
        var (_, _, gamma, _, epsilon) = await SeedAlphaHierarchyAsync();
        await SoftDeleteAsync(gamma);
        var deletedAtBefore = (await GetDepartmentsAsync())[gamma].DeletedAt;

        var result = await BulkSoftDeleteAsync(gamma, epsilon);

        Assert.True(result.IsSuccess);

        var departments = await GetDepartmentsAsync();

        Assert.Equal(deletedAtBefore, departments[gamma].DeletedAt);
        Assert.NotNull(departments[epsilon].DeletedAt);
    }

    [Fact]
    public async Task BulkSoftDelete_With_Missing_Id_Should_Fail_And_Change_Nothing()
    {
        var (alpha, beta, gamma, delta, epsilon) = await SeedAlphaHierarchyAsync();
        var missingId = Guid.NewGuid();

        var result = await BulkSoftDeleteAsync(beta, missingId);

        Assert.True(result.IsFailure);
        AssertConflictFor(result.Error, NotFoundCode, missingId);

        var departments = await GetDepartmentsAsync();

        foreach (var id in new[] { alpha, beta, gamma, delta, epsilon })
        {
            Assert.Null(departments[id].DeletedAt);
            Assert.True(departments[id].IsActive);
        }
    }

    [Fact]
    public async Task BulkSoftDelete_Should_Collect_Error_For_Each_Missing_Id()
    {
        var (alpha, _, _, _, _) = await SeedAlphaHierarchyAsync();
        var firstMissingId = Guid.NewGuid();
        var secondMissingId = Guid.NewGuid();

        var result = await BulkSoftDeleteAsync(alpha, firstMissingId, secondMissingId);

        Assert.True(result.IsFailure);
        Assert.Equal(2, result.Error.Count());
        AssertConflictFor(result.Error, NotFoundCode, firstMissingId);
        AssertConflictFor(result.Error, NotFoundCode, secondMissingId);
    }

    [Fact]
    public async Task Endpoint_Should_Return_200_And_Archive_Department()
    {
        var departmentId = await CreateDepartmentAsync("Solo", "solo");

        var response = await PostAsync(departmentId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull((await GetDepartmentsAsync())[departmentId].DeletedAt);
    }

    [Fact]
    public async Task Endpoint_With_Missing_Id_Should_Return_409_And_Change_Nothing()
    {
        var departmentId = await CreateDepartmentAsync("Solo", "solo");

        var response = await PostAsync(departmentId, Guid.NewGuid());

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Null((await GetDepartmentsAsync())[departmentId].DeletedAt);
    }

    private static void AssertConflictFor(Errors errors, string code, Guid departmentId) =>
        Assert.Contains(
            errors,
            e => e.Code == code && e.Type == ErrorType.CONFLICT && e.InvalidField == departmentId.ToString());

    private Task<HttpResponseMessage> PostAsync(params Guid[] departmentIds) =>
        _client.PostAsJsonAsync("/api/departments/bulk/soft-delete", new BulkDepartmentsDto(departmentIds.ToList()));

    private Task<Result<Unit, Errors>> BulkSoftDeleteAsync(params Guid[] departmentIds) =>
        ExecuteHandler<BulkSoftDeleteCommand, Unit>(async sut =>
            await sut.Handle(new BulkSoftDeleteCommand(departmentIds.ToList()), CancellationToken.None));

    private Task<Result<Unit, Errors>> SoftDeleteAsync(Guid departmentId) =>
        ExecuteHandler<SoftDeleteDepartmentCommand, Unit>(async sut =>
            await sut.Handle(new SoftDeleteDepartmentCommand(new DepartmentId(departmentId)), CancellationToken.None));

    private Task<Dictionary<Guid, Department>> GetDepartmentsAsync() =>
        ExecuteInDb(async db => await db.Departments
            .IgnoreQueryFilters()
            .ToDictionaryAsync(d => d.Id.Id));
}
