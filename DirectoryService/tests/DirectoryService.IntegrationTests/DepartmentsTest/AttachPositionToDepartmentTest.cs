using DirectoryService.Application.Departments.Commands.AttachPositionToDepartment;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class AttachPositionToDepartmentTest : DirectoryBaseTests
{
    public AttachPositionToDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Attach_Position_To_Department_Should_Be_Successful()
    {
        var positionId = await CreatePositionAsync("Engineer");
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var command = new AttachPositionToDepartmentCommand(departmentId, positionId);

        var handlerResult = await ExecuteHandler<AttachPositionToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var departmentPosition =
                await db.DepartmentPositions.FirstOrDefaultAsync(
                    a => a.DepartmentId == new DepartmentId(command.DepartmentId),
                    cancellationToken);

            Assert.NotNull(departmentPosition);
            Assert.IsType<DepartmentPosition>(departmentPosition);
            Assert.Equal(departmentId, departmentPosition.DepartmentId.Id);
            Assert.Equal(positionId, departmentPosition.PositionId.Id);
        });
    }

    [Fact]
    public async Task Attach_Position_To_Department_Twice_Should_Fail()
    {
        var positionId = await CreatePositionAsync("Engineer");
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var command = new AttachPositionToDepartmentCommand(departmentId, positionId);

        var firstAttempt = await ExecuteHandler<AttachPositionToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));
        Assert.True(firstAttempt.IsSuccess);

        var secondAttempt = await ExecuteHandler<AttachPositionToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(secondAttempt.IsFailure);
    }

    [Fact]
    public async Task Attach_Position_To_Nonexistent_Department_Should_Fail()
    {
        var positionId = await CreatePositionAsync("Engineer");
        var cancellationToken = CancellationToken.None;

        var command = new AttachPositionToDepartmentCommand(Guid.NewGuid(), positionId);

        var handlerResult = await ExecuteHandler<AttachPositionToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }

    [Fact]
    public async Task Attach_Nonexistent_Position_To_Department_Should_Fail()
    {
        var departmentId = await CreateDepartmentAsync("TestNameDep", "Department");
        var cancellationToken = CancellationToken.None;

        var command = new AttachPositionToDepartmentCommand(departmentId, Guid.NewGuid());

        var handlerResult = await ExecuteHandler<AttachPositionToDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }
}
