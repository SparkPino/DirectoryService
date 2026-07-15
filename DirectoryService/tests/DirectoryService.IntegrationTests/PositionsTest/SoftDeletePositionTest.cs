using DirectoryService.Application.Positions.SoftDeletePosition;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.PositionsTest;

public class SoftDeletePositionTest : DirectoryBaseTests
{
    public SoftDeletePositionTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task SoftDelete_Position_Should_Keep_Row_And_Mark_DeletedAt()
    {
        var positionId = await CreatePositionAsync("SoftDeletePositionName");
        var cancellationToken = CancellationToken.None;

        var command = new SoftDeletePositionCommand(new PositionId(positionId));
        var handlerResult = await ExecuteHandler<SoftDeletePositionCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var position = await db.Positions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == new PositionId(positionId), cancellationToken);

            Assert.NotNull(position);
            Assert.False(position!.IsActive);
            Assert.NotEqual(default, position.DeletedAt);
        });
    }

    [Fact]
    public async Task SoftDelete_Position_Should_Be_Hidden_From_Default_Query()
    {
        var positionId = await CreatePositionAsync("SoftDeletePositionName2");
        var cancellationToken = CancellationToken.None;

        await ExecuteHandler<SoftDeletePositionCommand, Guid>(async sut =>
            await sut.Handle(new SoftDeletePositionCommand(new PositionId(positionId)), cancellationToken));

        await ExecuteInDb(async db =>
        {
            var position = await db.Positions
                .FirstOrDefaultAsync(p => p.Id == new PositionId(positionId), cancellationToken);

            Assert.Null(position);
        });
    }

    [Fact]
    public async Task SoftDelete_Nonexistent_Position_Should_Fail()
    {
        var cancellationToken = CancellationToken.None;

        var command = new SoftDeletePositionCommand(new PositionId(Guid.NewGuid()));
        var handlerResult = await ExecuteHandler<SoftDeletePositionCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }
}
