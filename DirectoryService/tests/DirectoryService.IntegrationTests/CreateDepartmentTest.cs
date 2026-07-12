using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Commands.AddDepartment;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.IntegrationTests;

public class CreateDepartmentTest : DirectoryBaseTests
{
    public CreateDepartmentTest(DirectoryServiceWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateDepartment_with_valid_data_should_succeed()
    {
        // Arrange
        Guid guidLocationId = await CreateLocationAsync("StuStaisy");
        var cancellationToken = CancellationToken.None;

        // Act
        var handlerResult = await ExecuteHandler<AddDepartmentCommand, Guid>(async sut =>
        {
            var command = new AddDepartmentCommand(new DepartmentDto(
                "Test",
                "Test",
                [guidLocationId],
                0,
                null));

            var a = await sut.Handle(command, cancellationToken);
            return a;
        });

        // Assert
        Assert.True(handlerResult.IsSuccess);
        Assert.NotEqual(Guid.Empty, handlerResult.Value);

        await ExecuteInDb(async dbcontext =>
        {
            var department = await dbcontext.Departments
                .Include(d => d.DepartmentsLocations)
                .FirstAsync(d => d.Id == new DepartmentId(handlerResult.Value), cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(department.Id.Id, handlerResult.Value);
            Assert.Contains(department.DepartmentsLocations, l => l.LocationId.Id == guidLocationId);
        });
    }

    [Fact]
    public async Task CreateDepartment_with_invalid_data_should_failure()
    {
        //Arrange
        var invalidDepartment = new DepartmentDto(
            "Test",
            "Test",
            [],
            0,
            null);
        var cancellationToken = CancellationToken.None;

        //Act
        var handlerResult = await ExecuteHandler<AddDepartmentCommand, Guid>(async sut =>
        {
            var command = new AddDepartmentCommand(invalidDepartment);

            var a = await sut.Handle(command, cancellationToken);
            return a;
        });

        //Assert
        Assert.True(handlerResult.IsFailure);
        Assert.False(handlerResult.IsSuccess);

        Assert.IsType<Errors>(handlerResult.Error);

        await ExecuteInReadDbContext(async db =>
        {
            var result = await db.ReadDepartments.FirstOrDefaultAsync(cancellationToken);
            Assert.Null(result);
        });
    }

    [Fact]
    public async Task CreateDepartment_with_nonexistent_location_should_failure()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var handlerResult = await ExecuteHandler<AddDepartmentCommand, Guid>(async sut =>
        {
            var command = new AddDepartmentCommand(new DepartmentDto(
                "Test",
                "Test",
                [Guid.NewGuid()],
                0,
                null));

            return await sut.Handle(command, cancellationToken);
        });

        // Assert
        Assert.True(handlerResult.IsFailure);

        await ExecuteInReadDbContext(async db =>
        {
            var result = await db.ReadDepartments.FirstOrDefaultAsync(cancellationToken);
            Assert.Null(result);
        });
    }
}