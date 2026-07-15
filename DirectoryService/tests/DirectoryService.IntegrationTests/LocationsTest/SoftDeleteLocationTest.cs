using DirectoryService.Application.Locations.Commands.SoftDeleteLocation;
using DirectoryService.Application.Locations.Queries.GetByIdLocation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.LocationsTest;

public class SoftDeleteLocationTest : DirectoryBaseTests
{
    public SoftDeleteLocationTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task SoftDelete_Location_Should_Keep_Row_And_Mark_DeletedAt()
    {
        var locationId = await CreateLocationAsync("SoftDeleteLocationName");
        var cancellationToken = CancellationToken.None;

        var command = new SoftDeleteLocationCommand(new LocationId(locationId));
        var handlerResult = await ExecuteHandler<SoftDeleteLocationCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var location = await db.Locations
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == new LocationId(locationId), cancellationToken);

            Assert.NotNull(location);
            Assert.False(location!.IsActive);
            Assert.NotEqual(default, location.DeletedAt);
        });
    }

    [Fact]
    public async Task SoftDelete_Location_Should_Be_Hidden_From_Default_Query()
    {
        var locationId = await CreateLocationAsync("SoftDeleteLocationName2");
        var cancellationToken = CancellationToken.None;

        await ExecuteHandler<SoftDeleteLocationCommand, Guid>(async sut =>
            await sut.Handle(new SoftDeleteLocationCommand(new LocationId(locationId)), cancellationToken));

        await ExecuteInDb(async db =>
        {
            var location = await db.Locations
                .FirstOrDefaultAsync(l => l.Id == new LocationId(locationId), cancellationToken);

            Assert.Null(location);
        });
    }

    [Fact]
    public async Task SoftDelete_Location_Should_Be_Hidden_From_GetById()
    {
        var locationId = await CreateLocationAsync("SoftDeleteLocationName3");
        var cancellationToken = CancellationToken.None;

        await ExecuteHandler<SoftDeleteLocationCommand, Guid>(async sut =>
            await sut.Handle(new SoftDeleteLocationCommand(new LocationId(locationId)), cancellationToken));

        var result = await ExecuteQueryHandler<GetByIdLocationQuery, LocationResponseDto>(async sut =>
            await sut.Handle(new GetByIdLocationQuery(new LocationId(locationId)), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Code == "location.not_found");
    }

    [Fact]
    public async Task SoftDelete_Nonexistent_Location_Should_Fail()
    {
        var cancellationToken = CancellationToken.None;

        var command = new SoftDeleteLocationCommand(new LocationId(Guid.NewGuid()));
        var handlerResult = await ExecuteHandler<SoftDeleteLocationCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }
}
