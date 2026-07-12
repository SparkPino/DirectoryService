using DirectoryService.Application.Departments.Commands.AttachLocationToDepartment;
using DirectoryService.Application.Departments.Commands.DetachLocationFromDepartment;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.IntegrationTests;

public class DetachLocationFromDepartmentTest : DirectoryBaseTests
{
    public DetachLocationFromDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Detach_Location_From_Department_Should_Be_Successful()
    {
        var locationId = await CreateLocationAsync("Kartoszka");
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var attachResult = await ExecuteHandler<AttachLocationToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(new AttachLocationToDepartmentCommand(departmentId, locationId), cancellationToken));
        Assert.True(attachResult.IsSuccess);

        var command = new DetachLocationFromDepartmentCommand(departmentId, locationId);
        var handlerResult = await ExecuteHandler<DetachLocationFromDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var departmentLocation = await db.DepartmentLocations.FirstOrDefaultAsync(
                a => a.DepartmentId == new DepartmentId(departmentId) && a.LocationId == new LocationId(locationId),
                cancellationToken);

            Assert.Null(departmentLocation);
        });
    }

    [Fact]
    public async Task Detach_Location_From_Department_Should_Fail_When_Not_Attached()
    {
        var locationId = await CreateLocationAsync("Kartoszka");
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var command = new DetachLocationFromDepartmentCommand(departmentId, locationId);
        var handlerResult = await ExecuteHandler<DetachLocationFromDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error, e => e.Code == "department.location.not_attached");
    }

    [Fact]
    public async Task Detach_Location_From_Nonexistent_Department_Should_Fail()
    {
        var locationId = await CreateLocationAsync("Kartoszka");
        var cancellationToken = CancellationToken.None;

        var command = new DetachLocationFromDepartmentCommand(Guid.NewGuid(), locationId);
        var handlerResult = await ExecuteHandler<DetachLocationFromDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }

    [Fact]
    public async Task Detach_Nonexistent_Location_From_Department_Should_Fail()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var command = new DetachLocationFromDepartmentCommand(departmentId, Guid.NewGuid());
        var handlerResult = await ExecuteHandler<DetachLocationFromDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }
}
