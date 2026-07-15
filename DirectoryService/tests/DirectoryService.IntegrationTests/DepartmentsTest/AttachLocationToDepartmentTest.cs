using DirectoryService.Application.Departments.Commands.AttachLocationToDepartment;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class AttachLocationToDepartmentTest : DirectoryBaseTests
{
    public AttachLocationToDepartmentTest(DirectoryServiceWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Attach_Location_To_Department_Should_Be_Successful()
    {
        var locationId = await CreateLocationAsync("Kartoszka");
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");

        var cancellationToken = CancellationToken.None;
        //Arrange 
        var command = new AttachLocationToDepartmentCommand(departmentId, locationId);
        //Act
        var handlerResu = await ExecuteHandler<AttachLocationToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        //Assert
        Assert.True(handlerResu.IsSuccess);
        Assert.False(handlerResu.IsFailure);

        await ExecuteInDb(async db =>
        {
            var departmentLocation =
                await db.DepartmentLocations.FirstOrDefaultAsync(
                    a => a.DepartmentId == new DepartmentId(command.DepartmentId),
                    cancellationToken);

            Assert.NotNull(departmentLocation);
            Assert.IsType<DepartmentLocation>(departmentLocation);
            Assert.Equal(departmentId, departmentLocation.DepartmentId.Id);
            Assert.Equal(locationId, departmentLocation.LocationId.Id);
        });
    }

    [Fact]
    public async Task Attach_Location_To_Department_Twice_Should_Fail()
    {
        var locationId = await CreateLocationAsync("Kartoszka");
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var command = new AttachLocationToDepartmentCommand(departmentId, locationId);

        var firstAttempt = await ExecuteHandler<AttachLocationToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));
        Assert.True(firstAttempt.IsSuccess);

        var secondAttempt = await ExecuteHandler<AttachLocationToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(secondAttempt.IsFailure);
    }

    [Fact]
    public async Task Attach_Location_To_Nonexistent_Department_Should_Fail()
    {
        var locationId = await CreateLocationAsync("Kartoszka");
        var cancellationToken = CancellationToken.None;

        var command = new AttachLocationToDepartmentCommand(Guid.NewGuid(), locationId);

        var handlerResu = await ExecuteHandler<AttachLocationToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResu.IsFailure);
    }

    [Fact]
    public async Task Attach_Nonexistent_Location_To_Department_Should_Fail()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var command = new AttachLocationToDepartmentCommand(departmentId, Guid.NewGuid());

        var handlerResu = await ExecuteHandler<AttachLocationToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResu.IsFailure);
    }
}