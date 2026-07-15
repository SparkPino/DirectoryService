using DirectoryService.Application.Departments.Commands.UpdateDepartment;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class UpdateDepartmentTest : DirectoryBaseTests
{
    public UpdateDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Update_Department_Name_Should_Succeed()
    {
        var departmentId = await CreateDepartmentAsync("Original", "Original");
        var cancellationToken = CancellationToken.None;
        var rowVersion = await GetRowVersionAsync(departmentId);

        var command = new UpdateDepartmentCommand(new UpdateDepartmentDto("Renamed", null, rowVersion), departmentId);
        var handlerResult = await ExecuteHandler<UpdateDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var department = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(departmentId), cancellationToken);

            Assert.Equal("Renamed", department.Name.Value);
            Assert.Equal("Original", department.Identifier.Identifier);
        });
    }

    [Fact]
    public async Task Update_Department_Identifier_Should_Succeed()
    {
        var departmentId = await CreateDepartmentAsync("Original", "Original");
        var cancellationToken = CancellationToken.None;
        var rowVersion = await GetRowVersionAsync(departmentId);

        var command = new UpdateDepartmentCommand(new UpdateDepartmentDto(null, "Renamed", rowVersion), departmentId);
        var handlerResult = await ExecuteHandler<UpdateDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var department = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(departmentId), cancellationToken);

            Assert.Equal("Original", department.Name.Value);
            Assert.Equal("Renamed", department.Identifier.Identifier);
        });
    }

    [Fact]
    public async Task Update_Department_With_Both_Name_And_Identifier_Null_Should_Fail()
    {
        var departmentId = await CreateDepartmentAsync("Original", "Original");
        var cancellationToken = CancellationToken.None;
        var rowVersion = await GetRowVersionAsync(departmentId);

        var command = new UpdateDepartmentCommand(new UpdateDepartmentDto(null, null, rowVersion), departmentId);
        var handlerResult = await ExecuteHandler<UpdateDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error, e => e.Code == "update.department");
    }

    [Fact]
    public async Task Update_Department_With_Stale_RowVersion_Should_Fail()
    {
        var departmentId = await CreateDepartmentAsync("Original", "Original");
        var cancellationToken = CancellationToken.None;
        var staleRowVersion = await GetRowVersionAsync(departmentId);

        // симулируем параллельное изменение той же записи другим "пользователем"
        await ExecuteInDb(async db =>
        {
            var department = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(departmentId));
            department.UpdateDepartment(identifier: DepartmentIdentifier.Create("Concurrent").Value);
            await db.SaveChangesAsync();
        });

        var command = new UpdateDepartmentCommand(new UpdateDepartmentDto("Renamed", null, staleRowVersion), departmentId);
        var handlerResult = await ExecuteHandler<UpdateDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error, e => e.Code == "database.concurrency_conflict");
    }

    [Fact]
    public async Task Update_Nonexistent_Department_Should_Fail()
    {
        var cancellationToken = CancellationToken.None;

        var command = new UpdateDepartmentCommand(new UpdateDepartmentDto("Renamed", null, 0), Guid.NewGuid());
        var handlerResult = await ExecuteHandler<UpdateDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }

    [Fact]
    public async Task Update_Department_Identifier_Does_Not_Recompute_Path()
    {
        // Документирует текущее поведение: переименование Identifier у департамента
        // не пересчитывает его Path (он остаётся построенным по старому идентификатору).
        // Если это когда-нибудь исправят, тест нужно осознанно обновить.
        var parentId = await CreateDepartmentAsync("Parent", "Parent");
        var childId = await CreateDepartmentAsync("Child", "Child", parentId);
        var cancellationToken = CancellationToken.None;
        var rowVersion = await GetRowVersionAsync(childId);

        var command = new UpdateDepartmentCommand(new UpdateDepartmentDto(null, "Renamed", rowVersion), childId);
        var handlerResult = await ExecuteHandler<UpdateDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var child = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(childId), cancellationToken);

            Assert.Equal("Renamed", child.Identifier.Identifier);
            Assert.Equal("Parent.Child", child.Path.Path);
        });
    }

    private async Task<uint> GetRowVersionAsync(Guid departmentId)
    {
        return await ExecuteInDb(async db =>
        {
            var department = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(departmentId));
            return department.RowVersion;
        });
    }
}
