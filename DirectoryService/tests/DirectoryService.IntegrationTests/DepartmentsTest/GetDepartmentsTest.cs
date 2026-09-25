using DirectoryService.Application.Departments.Commands.AttachLocationToDepartment;
using DirectoryService.Application.Departments.Queries.GetDepartments;
using DirectoryService.Contracts.Department;
using SharedLibrary.SharedKernel;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class GetDepartmentsTest : DirectoryBaseTests
{
    public GetDepartmentsTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Search_Should_Be_Case_Insensitive_And_Return_Flat_Dto()
    {
        var financeId = await CreateDepartmentAsync("Finance", "finance");
        await CreateDepartmentAsync("Marketing", "marketing");
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<GetDepartmentsQuery, PagedResult<GetDepartmentDto>>(async sut =>
            await sut.Handle(new GetDepartmentsQuery { Search = "FINAN" }, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal(1, result.Value.TotalPage);

        // Контракт для DepartmentSelect: плоский DTO со всеми полями, нужными UI
        var department = Assert.Single(result.Value.Items);
        Assert.Equal(financeId, department.DepartmentId);
        Assert.Equal("Finance", department.Name);
        Assert.Equal("finance", department.Identifier);
        Assert.False(string.IsNullOrWhiteSpace(department.Path));
        Assert.True(department.IsActive);
        Assert.NotEqual(default, department.CreatedAt);
        Assert.Null(department.DeletedAt);
    }

    [Fact]
    public async Task ExcludeIds_Should_Remove_Departments_From_Result()
    {
        var firstId = await CreateDepartmentAsync("Sales North", "salesnorth");
        var secondId = await CreateDepartmentAsync("Sales South", "salessouth");
        var cancellationToken = CancellationToken.None;

        var query = new GetDepartmentsQuery { Search = "Sales", ExcludeIds = [firstId] };
        var result = await ExecuteQueryHandler<GetDepartmentsQuery, PagedResult<GetDepartmentDto>>(async sut =>
            await sut.Handle(query, cancellationToken));

        Assert.True(result.IsSuccess);
        var department = Assert.Single(result.Value.Items);
        Assert.Equal(secondId, department.DepartmentId);
    }

    [Fact]
    public async Task ParentId_Should_Return_Only_Direct_Children()
    {
        var (alpha, beta, _, _, epsilon) = await SeedAlphaHierarchyAsync();
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteQueryHandler<GetDepartmentsQuery, PagedResult<GetDepartmentDto>>(async sut =>
            await sut.Handle(new GetDepartmentsQuery { ParentId = alpha }, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(
            new[] { beta, epsilon }.Order(),
            result.Value.Items.Select(d => d.DepartmentId).Order());
    }

    [Fact]
    public async Task LocationIds_Should_Return_Linked_Departments_Without_Duplicates()
    {
        var firstLocationId = await CreateLocationAsync("Warsaw Office");
        var secondLocationId = await CreateLocationAsync("Krakow Office");
        var linkedId = await CreateDepartmentAsync("Linked", "linked");
        await CreateDepartmentAsync("Not Linked", "notlinked");
        var cancellationToken = CancellationToken.None;

        // Привязываем подразделение к обеим локациям, чтобы проверить отсутствие дублей
        foreach (var locationId in new[] { firstLocationId, secondLocationId })
        {
            var attachResult = await ExecuteHandler<AttachLocationToDepartmentCommand, Guid>(async sut =>
                await sut.Handle(new AttachLocationToDepartmentCommand(linkedId, locationId), cancellationToken));
            Assert.True(attachResult.IsSuccess);
        }

        var query = new GetDepartmentsQuery { LocationIds = [firstLocationId, secondLocationId] };
        var result = await ExecuteQueryHandler<GetDepartmentsQuery, PagedResult<GetDepartmentDto>>(async sut =>
            await sut.Handle(query, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.TotalCount);
        var department = Assert.Single(result.Value.Items);
        Assert.Equal(linkedId, department.DepartmentId);
    }
}
