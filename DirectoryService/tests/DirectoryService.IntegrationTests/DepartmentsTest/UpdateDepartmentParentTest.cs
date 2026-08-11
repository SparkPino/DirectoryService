using DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;
using DirectoryService.Application.Departments.Commands.UpdateDepartmentParent;
using DirectoryService.Contracts.Department.UpdateDepartmentParent;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class UpdateDepartmentParentTest : DirectoryBaseTests
{
    public UpdateDepartmentParentTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task UpdateParent_HappyPath_With_Descendants_Should_Recalculate_Path_And_Depth()
    {
        var (alphaId, betaId, gammaId, deltaId, epsilonId) = await SeedAlphaHierarchyAsync();
        var omegaId = await CreateDepartmentAsync("Omega", "omega");
        var cancellationToken = CancellationToken.None;

        var command = new UpdateDepartmentParentCommand(betaId, new UpdateDepartmentParentParentDto(omegaId));
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(omegaId, handlerResult.Value.ParentId);
        Assert.Equal("omega.beta", handlerResult.Value.Path);
        Assert.Equal(1, handlerResult.Value.Depth);

        await ExecuteInDb(async db =>
        {
            var beta = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(betaId), cancellationToken);
            Assert.Equal(new DepartmentId(omegaId), beta.ParentId);
            Assert.Equal((short)1, beta.Depth);
            Assert.Equal("omega.beta", beta.Path.Path);

            var gamma = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(gammaId), cancellationToken);
            Assert.Equal("omega.beta.gamma", gamma.Path.Path);
            Assert.Equal((short)2, gamma.Depth);

            var delta = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(deltaId), cancellationToken);
            Assert.Equal("omega.beta.delta", delta.Path.Path);
            Assert.Equal((short)2, delta.Depth);

            // epsilon - сиблинг beta под alpha, переезда beta его не должен касаться
            var epsilon = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(epsilonId), cancellationToken);
            Assert.Equal("alpha.epsilon", epsilon.Path.Path);
            Assert.Equal((short)1, epsilon.Depth);
        });
    }

    [Fact]
    public async Task UpdateParent_To_Root_When_ParentId_Is_Null_Should_Succeed()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "alpha");
        var betaId = await CreateDepartmentAsync("Beta", "beta", alphaId);
        var cancellationToken = CancellationToken.None;

        var command = new UpdateDepartmentParentCommand(betaId, new UpdateDepartmentParentParentDto(null));
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);
        Assert.Null(handlerResult.Value.ParentId);
        Assert.Equal("beta", handlerResult.Value.Path);
        Assert.Equal(0, handlerResult.Value.Depth);

        await ExecuteInDb(async db =>
        {
            var beta = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(betaId), cancellationToken);
            Assert.Null(beta.ParentId);
            Assert.Equal((short)0, beta.Depth);
            Assert.Equal("beta", beta.Path.Path);
        });
    }

    [Fact]
    public async Task UpdateParent_With_Null_ParentId_Wrapper_Should_Be_Treated_As_Root()
    {
        // Тут ParentId - сама обёртка UpdateDepartmentParentParentDto равна null,
        // а не new UpdateDepartmentParentParentDto(null) (где обёртка есть, а null - только Id внутри).
        // Раньше RuleFor(ud => ud.ParentId.Id) падал с NullReferenceException именно на этом кейсе,
        // потому что селектор пытался достать .Id у null-обёртки ещё до того, как отработает Must.
        var alphaId = await CreateDepartmentAsync("Alpha", "alpha");
        var betaId = await CreateDepartmentAsync("Beta", "beta", alphaId);
        var cancellationToken = CancellationToken.None;

        var command = new UpdateDepartmentParentCommand(betaId, null);
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);
        Assert.Null(handlerResult.Value.ParentId);
        Assert.Equal("beta", handlerResult.Value.Path);
        Assert.Equal(0, handlerResult.Value.Depth);

        await ExecuteInDb(async db =>
        {
            var beta = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(betaId), cancellationToken);
            Assert.Null(beta.ParentId);
            Assert.Equal((short)0, beta.Depth);
            Assert.Equal("beta", beta.Path.Path);
        });
    }

    [Fact]
    public async Task UpdateParent_Into_Own_Descendant_Should_Return_Cycle_Conflict()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "alpha");
        var betaId = await CreateDepartmentAsync("Beta", "beta", alphaId);
        var gammaId = await CreateDepartmentAsync("Gamma", "gamma", betaId);
        var cancellationToken = CancellationToken.None;

        var command = new UpdateDepartmentParentCommand(alphaId, new UpdateDepartmentParentParentDto(gammaId));
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error, e => e.Code == "department.move.cycle" && e.Type == ErrorType.CONFLICT);

        await ExecuteInDb(async db =>
        {
            var alpha = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(alphaId), cancellationToken);
            Assert.Null(alpha.ParentId);
            Assert.Equal((short)0, alpha.Depth);
            Assert.Equal("alpha", alpha.Path.Path);
        });
    }

    [Fact]
    public async Task UpdateParent_To_Self_Should_Return_ParentIsSelf_Validation()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "alpha");
        var cancellationToken = CancellationToken.None;

        var command = new UpdateDepartmentParentCommand(alphaId, new UpdateDepartmentParentParentDto(alphaId));
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error,
            e => e.Code == "department.move.parent_is_self" && e.Type == ErrorType.VALIDATION);
    }

    [Fact]
    public async Task UpdateParent_To_SoftDeleted_Parent_Should_Return_ParentDeleted_Conflict()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "alpha");
        var betaId = await CreateDepartmentAsync("Beta", "beta");
        var cancellationToken = CancellationToken.None;

        var softDeleteResult = await ExecuteHandler<SoftDeleteDepartmentCommand, Guid>(async sut =>
            await sut.Handle(new SoftDeleteDepartmentCommand(new DepartmentId(alphaId)), cancellationToken));
        Assert.True(softDeleteResult.IsSuccess);

        var command = new UpdateDepartmentParentCommand(betaId, new UpdateDepartmentParentParentDto(alphaId));
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error,
            e => e.Code == "department.move.parent_deleted" && e.Type == ErrorType.CONFLICT);

        await ExecuteInDb(async db =>
        {
            var beta = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(betaId), cancellationToken);
            Assert.Null(beta.ParentId);
            Assert.Equal("beta", beta.Path.Path);
        });
    }

    [Fact]
    public async Task UpdateParent_Nonexistent_Department_Should_Return_DepartmentNotFound()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "alpha");
        var cancellationToken = CancellationToken.None;

        var command = new UpdateDepartmentParentCommand(Guid.NewGuid(), new UpdateDepartmentParentParentDto(alphaId));
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error, e => e.Code == "department.not_found" && e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task UpdateParent_To_Nonexistent_Parent_Should_Return_ParentNotFound()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "alpha");
        var cancellationToken = CancellationToken.None;

        var command = new UpdateDepartmentParentCommand(alphaId, new UpdateDepartmentParentParentDto(Guid.NewGuid()));
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsFailure);
        Assert.Contains(handlerResult.Error,
            e => e.Code == "department.parent.not_found" && e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task UpdateParent_With_Same_Parent_Should_Be_NoOp_And_Not_Change_UpdatedAt()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "alpha");
        var betaId = await CreateDepartmentAsync("Beta", "beta", alphaId);
        var gammaId = await CreateDepartmentAsync("Gamma", "gamma", betaId);
        var cancellationToken = CancellationToken.None;

        DateTimeOffset? betaUpdatedAtBefore = null;
        DateTimeOffset? gammaUpdatedAtBefore = null;
        await ExecuteInDb(async db =>
        {
            var beta = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(betaId), cancellationToken);
            betaUpdatedAtBefore = beta.UpdatedAt;

            var gamma = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(gammaId), cancellationToken);
            gammaUpdatedAtBefore = gamma.UpdatedAt;
        });

        // тот же родитель, что и сейчас - должно завершиться как no-op
        var command = new UpdateDepartmentParentCommand(betaId, new UpdateDepartmentParentParentDto(alphaId));
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(alphaId, handlerResult.Value.ParentId);
        Assert.Equal("alpha.beta", handlerResult.Value.Path);
        Assert.Equal(betaUpdatedAtBefore, handlerResult.Value.UpdatedAt);

        await ExecuteInDb(async db =>
        {
            var beta = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(betaId), cancellationToken);
            Assert.Equal(betaUpdatedAtBefore, beta.UpdatedAt);
            Assert.Equal((short)1, beta.Depth);
            Assert.Equal("alpha.beta", beta.Path.Path);

            // потомок тоже не должен быть тронут повторным bulk-update
            var gamma = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(gammaId), cancellationToken);
            Assert.Equal(gammaUpdatedAtBefore, gamma.UpdatedAt);
        });
    }

    [Fact]
    public async Task UpdateParent_Root_To_Root_Should_Be_NoOp()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "alpha");
        var cancellationToken = CancellationToken.None;

        DateTimeOffset? updatedAtBefore = null;
        await ExecuteInDb(async db =>
        {
            var alpha = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(alphaId), cancellationToken);
            updatedAtBefore = alpha.UpdatedAt;
        });

        var command = new UpdateDepartmentParentCommand(alphaId, new UpdateDepartmentParentParentDto(null));
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);
        Assert.Null(handlerResult.Value.ParentId);
        Assert.Equal(updatedAtBefore, handlerResult.Value.UpdatedAt);
    }

    [Fact]
    public async Task UpdateParent_Large_Subtree_Should_Update_All_Descendants_In_One_Bulk_Statement()
    {
        const int childrenCount = 50;

        var bigRootId = await CreateDepartmentAsync("BigRoot", "bigroot");

        var childIds = new List<Guid>();
        for (var i = 0; i < childrenCount; i++)
        {
            var childId = await CreateDepartmentAsync($"Child{i}", ToLetterIdentifier(i), bigRootId);
            childIds.Add(childId);
        }

        var newHomeId = await CreateDepartmentAsync("NewHome", "newhome");
        var cancellationToken = CancellationToken.None;

        var command = new UpdateDepartmentParentCommand(bigRootId, new UpdateDepartmentParentParentDto(newHomeId));

        // сам факт, что перенос поддерева из childrenCount узлов выполняется одним вызовом
        // хендлера без цикла по потомкам на стороне C#, обеспечивается реализацией
        // (один UpdateBySqlAsync на всё поддерево) - здесь же проверяем именно результат:
        // что каждый потомок реально получил новый path/depth после этого единственного вызова.
        var handlerResult = await ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(command, cancellationToken));

        Assert.True(handlerResult.IsSuccess);
        Assert.Equal("newhome.bigroot", handlerResult.Value.Path);
        Assert.Equal(1, handlerResult.Value.Depth);

        await ExecuteInDb(async db =>
        {
            foreach (var childId in childIds)
            {
                var child = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(childId), cancellationToken);
                Assert.StartsWith("newhome.bigroot.", child.Path.Path);
                Assert.Equal((short)2, child.Depth);
            }
        });
    }

    // Конкурентные тесты (DS-25): запросы отправляются реально параллельно через Task.WhenAll -
    // ExecuteHandler открывает под каждый вызов свой DI-scope, а значит свой DbContext/соединение,
    // поэтому это два независимых конкурентных запроса к одному и тому же Postgres из Testcontainers,
    // а не имитация последовательного вызова.

    [Fact]
    public async Task UpdateParent_Concurrently_ToTwoDifferentParents_Should_KeepTreeConsistent()
    {
        var nodeAId = await CreateDepartmentAsync("NodeA", "nodea");
        var childOfAId = await CreateDepartmentAsync("ChildOfA", "childofa", nodeAId);
        var targetXId = await CreateDepartmentAsync("TargetX", "targetx");
        var targetYId = await CreateDepartmentAsync("TargetY", "targety");
        var cancellationToken = CancellationToken.None;

        var moveUnderX = new UpdateDepartmentParentCommand(nodeAId, new UpdateDepartmentParentParentDto(targetXId));
        var moveUnderY = new UpdateDepartmentParentCommand(nodeAId, new UpdateDepartmentParentParentDto(targetYId));

            // Асинхронные методы без await не ждут выполнения друг друга и стартуют по очереди без ожидания завершения, WhenAll ждет результат завершения каждого.
        var task1 = ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(moveUnderX, cancellationToken));
        var task2 = ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(moveUnderY, cancellationToken));

        var results = await Task.WhenAll(task1, task2);

        // общий anchor у обоих запросов только nodeA - deadlock тут невозможен по конструкции,
        // проигравший просто ждёт lock на nodeA и продолжает уже на свежих данных
        var diagnostic = string.Join(
            " | ",
            results.Select(r => r.IsSuccess
                ? "success"
                : string.Join(",", r.Error.Select(e => $"{e.Code}:{e.Message}:{e.Type}"))));
        Assert.All(results, r => Assert.True(r.IsSuccess, diagnostic));

        await ExecuteInDb(async db =>
        {
            var nodeA = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(nodeAId), cancellationToken);

            var targetXDepartmentId = new DepartmentId(targetXId);
            var targetYDepartmentId = new DepartmentId(targetYId);
            Assert.True(
                nodeA.ParentId == targetXDepartmentId || nodeA.ParentId == targetYDepartmentId,
                "после двух конкурентных переносов узел должен оказаться ровно под одним из двух родителей");

            var winningParent = await db.Departments.FirstAsync(
                d => d.Id == nodeA.ParentId!, cancellationToken);

            Assert.Equal($"{winningParent.Path.Path}.nodea", nodeA.Path.Path);
            Assert.Equal((short)(winningParent.Depth + 1), nodeA.Depth);

            // потомок nodeA должен быть пересчитан от актуального (а не от промежуточного) состояния узла
            var childOfA = await db.Departments.FirstAsync(
                d => d.Id == new DepartmentId(childOfAId), cancellationToken);
            Assert.Equal($"{nodeA.Path.Path}.childofa", childOfA.Path.Path);
            Assert.Equal((short)(nodeA.Depth + 1), childOfA.Depth);
        });
    }

    [Fact]
    public async Task UpdateParent_Concurrently_MutualSwap_Should_RejectOneWithCycleConflict()
    {
        var aId = await CreateDepartmentAsync("SwapA", "swapa");
        var bId = await CreateDepartmentAsync("SwapB", "swapb");
        var cancellationToken = CancellationToken.None;

        var moveAUnderB = new UpdateDepartmentParentCommand(aId, new UpdateDepartmentParentParentDto(bId));
        var moveBUnderA = new UpdateDepartmentParentCommand(bId, new UpdateDepartmentParentParentDto(aId));

        var task1 = ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(moveAUnderB, cancellationToken));
        var task2 = ExecuteHandler<UpdateDepartmentParentCommand, DepartmentParentDto>(async sut =>
            await sut.Handle(moveBUnderA, cancellationToken));

        var results = await Task.WhenAll(task1, task2);

        // anchor-ы обоих запросов - одна и та же пара {A, B}, просто в разных ролях, поэтому
        // порядок locks у них одинаковый - deadlock невозможен, а второй по очереди запрос
        // увидит уже актуальное состояние дерева и корректно поймает цикл
        Assert.Single(results, r => r.IsSuccess);
        var failed = Assert.Single(results, r => r.IsFailure);
        Assert.Contains(failed.Error, e => e.Code == "department.move.cycle" && e.Type == ErrorType.CONFLICT);

        await ExecuteInDb(async db =>
        {
            var a = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(aId), cancellationToken);
            var b = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(bId), cancellationToken);

            var aUnderB = a.ParentId == new DepartmentId(bId);
            var bUnderA = b.ParentId == new DepartmentId(aId);

            // ровно один из двух вариантов - если бы оба стали true, это и был бы цикл
            Assert.True(aUnderB ^ bUnderA);

            if (aUnderB)
            {
                Assert.StartsWith(b.Path.Path + ".", a.Path.Path);
                Assert.Equal((short)(b.Depth + 1), a.Depth);
            }
            else
            {
                Assert.StartsWith(a.Path.Path + ".", b.Path.Path);
                Assert.Equal((short)(a.Depth + 1), b.Depth);
            }
        });
    }

    private static string ToLetterIdentifier(int index)
    {
        var suffix = string.Empty;
        var n = index;
        do
        {
            suffix = (char)('a' + (n % 26)) + suffix;
            n = (n / 26) - 1;
        } while (n >= 0);

        return "child" + suffix;
    }
}
