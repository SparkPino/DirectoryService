using DirectoryService.Application.Departments.Commands.MoveDepartment;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class MoveDepartmentTest : DirectoryBaseTests
{
    public MoveDepartmentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Move_Department_To_New_Parent_Should_Update_Path_And_Depth()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "Alpha");
        var betaId = await CreateDepartmentAsync("Beta", "Beta");
        var cancellationToken = CancellationToken.None;

        var command = new MoveDepartmentCommand(betaId, alphaId);
        var handlerResult = await ExecuteHandler<MoveDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var beta = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(betaId), cancellationToken);

            Assert.Equal(new DepartmentId(alphaId), beta.ParentId);
            Assert.Equal((short)1, beta.Depth);
            Assert.Equal("Alpha.Beta", beta.Path.Path);
        });
    }

    [Fact]
    public async Task Move_Department_Should_Update_All_Descendants_Path_And_Depth()
    {
        var echoId = await CreateDepartmentAsync("Echo", "Echo");
        var deltaId = await CreateDepartmentAsync("Delta", "Delta", echoId);
        var betaId = await CreateDepartmentAsync("Beta", "Beta");
        var gammaId = await CreateDepartmentAsync("Gamma", "Gamma", betaId);
        var cancellationToken = CancellationToken.None;

        var command = new MoveDepartmentCommand(betaId, deltaId);
        var handlerResult = await ExecuteHandler<MoveDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var gamma = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(gammaId), cancellationToken);

            Assert.Equal("Echo.Delta.Beta.Gamma", gamma.Path.Path);
            Assert.Equal((short)3, gamma.Depth);
        });
    }

    [Fact]
    public async Task Move_Department_To_Root_When_NewParentId_Is_Null_Should_Succeed()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "Alpha");
        var betaId = await CreateDepartmentAsync("Beta", "Beta", alphaId);
        var cancellationToken = CancellationToken.None;

        var command = new MoveDepartmentCommand(betaId, null);
        var handlerResult = await ExecuteHandler<MoveDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);

        await ExecuteInDb(async db =>
        {
            var beta = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(betaId), cancellationToken);

            Assert.Null(beta.ParentId);
            Assert.Equal((short)0, beta.Depth);
            Assert.Equal("Beta", beta.Path.Path);
        });
    }

    [Fact]
    public async Task Move_Department_Into_Itself_Should_Fail()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "Alpha");
        var cancellationToken = CancellationToken.None;

        var command = new MoveDepartmentCommand(alphaId, alphaId);
        var handlerResult = await ExecuteHandler<MoveDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error, e => e.Code == "department.move.invalid_target");

        await ExecuteInDb(async db =>
        {
            var alpha = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(alphaId), cancellationToken);

            Assert.Null(alpha.ParentId);
            Assert.Equal((short)0, alpha.Depth);
        });
    }

    [Fact]
    public async Task Move_Department_Into_Own_Descendant_Should_Fail()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "Alpha");
        var betaId = await CreateDepartmentAsync("Beta", "Beta", alphaId);
        var cancellationToken = CancellationToken.None;

        var command = new MoveDepartmentCommand(alphaId, betaId);
        var handlerResult = await ExecuteHandler<MoveDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error, e => e.Code == "department.move.invalid_target");
    }

    [Fact]
    public async Task Move_Nonexistent_Department_Should_Fail()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "Alpha");
        var cancellationToken = CancellationToken.None;

        var command = new MoveDepartmentCommand(Guid.NewGuid(), alphaId);
        var handlerResult = await ExecuteHandler<MoveDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }

    [Fact]
    public async Task Move_Department_To_Nonexistent_NewParent_Should_Fail()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "Alpha");
        var cancellationToken = CancellationToken.None;

        var command = new MoveDepartmentCommand(alphaId, Guid.NewGuid());
        var handlerResult = await ExecuteHandler<MoveDepartmentCommand, Guid>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
    }
}
