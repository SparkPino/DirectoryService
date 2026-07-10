using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Departments.Commands.AddDepartment;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Shared.ValueObjects;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        Guid guidLocationId = await CreateLocation();
        var cancellationToken = CancellationToken.None;

        // Act
        var handlerResult = await ExecuteHandler(sut =>
        {
            var command = new AddDepartmentCommand(new DepartmentDto(
                "Test",
                "Test",
                [guidLocationId],
                0,
                null));

            return sut.Handle(command, cancellationToken);
        });

        // Assert
        // Assert.Empty(handlerResult.Error);
        await ExecuteInDb(async dbcontext =>
        {
            var department = await dbcontext.Departments
                .FirstAsync(d => d.Id == new DepartmentId(handlerResult.Value), cancellationToken);
            Assert.NotNull(department);
            Assert.Equal(department.Id.Id, handlerResult);
            Assert.True(handlerResult.IsSuccess);
            Assert.NotEqual(Guid.Empty, handlerResult.Value);
        });
    }

    private async Task<Guid> CreateLocation()
    {
        var locationNameResult = LocationName.Create("StuStaisy").Value;

        var locationAddressResult = Address.Create(
            "Poland",
            "Warsaw",
            "Szwedzka",
            "03-420",
            "30",
            "A").Value;

        var locationTimeZoneResult = LocationTimeZone.Create("Europe/Warsaw").Value;

        var location = Location.Create(locationNameResult, locationAddressResult, locationTimeZoneResult);

        return await ExecuteInDb(async dbContext =>
        {
            await dbContext.Locations.AddAsync(location);
            await dbContext.SaveChangesAsync();
            return location.Id.Id;
        });
    }
}