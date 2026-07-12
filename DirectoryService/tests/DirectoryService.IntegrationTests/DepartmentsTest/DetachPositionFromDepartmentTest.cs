using DirectoryService.Application.Departments.Commands.AttachPositionToDepartment;
using DirectoryService.Application.Departments.Commands.DetachPositionFromDepartment;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class DetachPositionFromDepartmentTest : DirectoryBaseTests
{
    public DetachPositionFromDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Detach_Position_From_Department_Should_Be_Successful()
    {
        var positionId = await CreatePositionAsync("Engineer");
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var attachResult = await ExecuteHandler<AttachPositionToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(new AttachPositionToDepartmentCommand(departmentId, positionId), cancellationToken));
        Assert.True(attachResult.IsSuccess);

        var command = new DetachPositionFromDepartmentCommand(departmentId, positionId);
        var handlerResult = await ExecuteHandler<DetachPositionFromDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var departmentPosition = await db.DepartmentPositions.FirstOrDefaultAsync(
                a => a.DepartmentId == new DepartmentId(departmentId) && a.PositionId == new PositionId(positionId),
                cancellationToken);

            Assert.Null(departmentPosition);
        });
    }

    [Fact]
    public async Task Detach_Position_From_Department_Should_Fail_When_Not_Attached()
    {
        var positionId = await CreatePositionAsync("Engineer");
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var command = new DetachPositionFromDepartmentCommand(departmentId, positionId);
        var handlerResult = await ExecuteHandler<DetachPositionFromDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error, e => e.Code == "department.position.not_attached");
    }

    [Fact]
    public async Task Detach_Position_From_Nonexistent_Department_Should_Fail()
    {
        var positionId = await CreatePositionAsync("Engineer");
        var cancellationToken = CancellationToken.None;

        var command = new DetachPositionFromDepartmentCommand(Guid.NewGuid(), positionId);
        var handlerResult = await ExecuteHandler<DetachPositionFromDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }

    [Fact]
    public async Task Detach_Nonexistent_Position_From_Department_Should_Fail()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var command = new DetachPositionFromDepartmentCommand(departmentId, Guid.NewGuid());
        var handlerResult = await ExecuteHandler<DetachPositionFromDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }
}
