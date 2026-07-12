using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Commands.AttachLocationToDepartment;
using DirectoryService.Application.Departments.Commands.AttachPositionToDepartment;
using DirectoryService.Application.Departments.Commands.RemoveDepartment;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.IntegrationTests;

public class RemoveDepartmentTest : DirectoryBaseTests
{
    public RemoveDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Remove_Department_Should_Be_Successful_HardDelete()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var command = new RemoveDepartmentCommand(departmentId);
        var handlerResult = await ExecuteHandler<RemoveDepartmentCommand, Unit>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var department = await db.Departments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(departmentId), cancellationToken);

            Assert.Null(department);
        });
    }

    [Fact]
    public async Task Remove_Department_With_Child_Departments_Should_Fail()
    {
        var parentId = await CreateDepartmentAsync("Parent", "Parent");
        await CreateDepartmentAsync("Child", "Child", parentId);
        var cancellationToken = CancellationToken.None;

        var command = new RemoveDepartmentCommand(parentId);
        var handlerResult = await ExecuteHandler<RemoveDepartmentCommand, Unit>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error, e => e.Code == "record.has.dependents");

        await ExecuteInDb(async db =>
        {
            var parent = await db.Departments.FirstOrDefaultAsync(
                d => d.Id == new DepartmentId(parentId),
                cancellationToken);

            Assert.NotNull(parent);
        });
    }

    [Fact]
    public async Task Remove_Department_With_Attached_Locations_Should_Succeed_And_Cascade_Delete_Join_Row()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var locationId = await CreateLocationAsync("Kartoszka");
        var cancellationToken = CancellationToken.None;

        var attachResult = await ExecuteHandler<AttachLocationToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(new AttachLocationToDepartmentCommand(departmentId, locationId), cancellationToken));
        Assert.True(attachResult.IsSuccess);

        var command = new RemoveDepartmentCommand(departmentId);
        var handlerResult = await ExecuteHandler<RemoveDepartmentCommand, Unit>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var departmentLocation = await db.DepartmentLocations.FirstOrDefaultAsync(
                a => a.DepartmentId == new DepartmentId(departmentId),
                cancellationToken);
            Assert.Null(departmentLocation);

            var location = await db.Locations.FirstOrDefaultAsync(
                l => l.Id == new LocationId(locationId),
                cancellationToken);
            Assert.NotNull(location);
        });
    }

    [Fact]
    public async Task Remove_Department_With_Attached_Positions_Should_Succeed_And_Cascade_Delete_Join_Row()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var positionId = await CreatePositionAsync("Engineer");
        var cancellationToken = CancellationToken.None;

        var attachResult = await ExecuteHandler<AttachPositionToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(new AttachPositionToDepartmentCommand(departmentId, positionId), cancellationToken));
        Assert.True(attachResult.IsSuccess);

        var command = new RemoveDepartmentCommand(departmentId);
        var handlerResult = await ExecuteHandler<RemoveDepartmentCommand, Unit>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var departmentPosition = await db.DepartmentPositions.FirstOrDefaultAsync(
                a => a.DepartmentId == new DepartmentId(departmentId),
                cancellationToken);
            Assert.Null(departmentPosition);

            var position = await db.Positions.FirstOrDefaultAsync(
                p => p.Id == new PositionId(positionId),
                cancellationToken);
            Assert.NotNull(position);
        });
    }

    [Fact]
    public async Task Remove_Nonexistent_Department_Should_Fail()
    {
        var cancellationToken = CancellationToken.None;

        var command = new RemoveDepartmentCommand(Guid.NewGuid());
        var handlerResult = await ExecuteHandler<RemoveDepartmentCommand, Unit>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }
}