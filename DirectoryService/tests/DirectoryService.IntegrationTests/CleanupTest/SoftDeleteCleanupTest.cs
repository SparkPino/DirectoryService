using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.SharedKernel;

namespace DirectoryService.IntegrationTests.CleanupTest;

public class SoftDeleteCleanupTest : DirectoryBaseTests
{
    public SoftDeleteCleanupTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Cleanup_Should_Physically_Delete_Expired_SoftDeleted_Department()
    {
        var departmentId = await CreateDepartmentAsync("ExpiredDep", "ExpiredDep");
        var cancellationToken = CancellationToken.None;

        await BackdateDepartmentDeletion(departmentId, TimeSpan.FromDays(10));

        var result = await ExecuteRepository<ISoftDeleteCleanupRepository, Result<int, Error>>(repo =>
            repo.DeleteExpiredDepartmentsBatchAsync(DateTimeOffset.UtcNow.AddDays(-1), 100, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);

        await ExecuteInDb(async db =>
        {
            var department = await db.Departments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(departmentId), cancellationToken);

            Assert.Null(department);
        });
    }

    [Fact]
    public async Task Cleanup_Should_Not_Delete_Recently_SoftDeleted_Department()
    {
        var departmentId = await CreateDepartmentAsync("RecentDep", "RecentDep");
        var cancellationToken = CancellationToken.None;

        await BackdateDepartmentDeletion(departmentId, TimeSpan.FromHours(1));

        var result = await ExecuteRepository<ISoftDeleteCleanupRepository, Result<int, Error>>(repo =>
            repo.DeleteExpiredDepartmentsBatchAsync(DateTimeOffset.UtcNow.AddDays(-1), 100, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);

        await ExecuteInDb(async db =>
        {
            var department = await db.Departments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(departmentId), cancellationToken);

            Assert.NotNull(department);
        });
    }

    [Fact]
    public async Task Cleanup_Should_Physically_Delete_Expired_SoftDeleted_Location()
    {
        var locationId = await CreateLocationAsync("ExpiredLoc");
        var cancellationToken = CancellationToken.None;

        await BackdateLocationDeletion(locationId, TimeSpan.FromDays(10));

        var result = await ExecuteRepository<ISoftDeleteCleanupRepository, Result<int, Error>>(repo =>
            repo.DeleteExpiredLocationsBatchAsync(DateTimeOffset.UtcNow.AddDays(-1), 100, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);

        await ExecuteInDb(async db =>
        {
            var location = await db.Locations
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == new LocationId(locationId), cancellationToken);

            Assert.Null(location);
        });
    }

    [Fact]
    public async Task Cleanup_Should_Physically_Delete_Expired_SoftDeleted_Position()
    {
        var positionId = await CreatePositionAsync("ExpiredPos");
        var cancellationToken = CancellationToken.None;

        await BackdatePositionDeletion(positionId, TimeSpan.FromDays(10));

        var result = await ExecuteRepository<ISoftDeleteCleanupRepository, Result<int, Error>>(repo =>
            repo.DeleteExpiredPositionsBatchAsync(DateTimeOffset.UtcNow.AddDays(-1), 100, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);

        await ExecuteInDb(async db =>
        {
            var position = await db.Positions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == new PositionId(positionId), cancellationToken);

            Assert.Null(position);
        });
    }

    [Fact]
    public async Task Cleanup_Should_Respect_BatchSize_Limit()
    {
        var cancellationToken = CancellationToken.None;

        for (var i = 0; i < 5; i++)
        {
            var suffix = ((char)('A' + i)).ToString();
            var id = await CreateDepartmentAsync($"BatchDep{suffix}", $"BatchDep{suffix}");
            await BackdateDepartmentDeletion(id, TimeSpan.FromDays(10));
        }

        var result = await ExecuteRepository<ISoftDeleteCleanupRepository, Result<int, Error>>(repo =>
            repo.DeleteExpiredDepartmentsBatchAsync(DateTimeOffset.UtcNow.AddDays(-1), 2, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public async Task Cleanup_Should_SetNull_ParentId_When_Parent_Deleted_While_Child_Still_Exists()
    {
        var cancellationToken = CancellationToken.None;
        var parentId = await CreateDepartmentAsync("ParentToDelete", "ParentToDelete");
        var childId = await CreateDepartmentAsync("ChildKept", "ChildKept", parentId);

        await BackdateDepartmentDeletion(parentId, TimeSpan.FromDays(10));

        var result = await ExecuteRepository<ISoftDeleteCleanupRepository, Result<int, Error>>(repo =>
            repo.DeleteExpiredDepartmentsBatchAsync(DateTimeOffset.UtcNow.AddDays(-1), 100, cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);

        await ExecuteInDb(async db =>
        {
            var parent = await db.Departments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(parentId), cancellationToken);
            Assert.Null(parent);

            var child = await db.Departments
                .FirstOrDefaultAsync(d => d.Id == new DepartmentId(childId), cancellationToken);
            Assert.NotNull(child);
            Assert.Null(child!.ParentId);
        });
    }

    private async Task BackdateDepartmentDeletion(Guid departmentId, TimeSpan age)
    {
        await ExecuteInDb(async db =>
        {
            var department = await db.Departments
                .IgnoreQueryFilters()
                .FirstAsync(d => d.Id == new DepartmentId(departmentId));

            department.SoftDelete();
            db.Entry(department).Property(d => d.DeletedAt).CurrentValue = DateTimeOffset.UtcNow - age;
            await db.SaveChangesAsync();
        });
    }

    private async Task BackdateLocationDeletion(Guid locationId, TimeSpan age)
    {
        await ExecuteInDb(async db =>
        {
            var location = await db.Locations
                .IgnoreQueryFilters()
                .FirstAsync(l => l.Id == new LocationId(locationId));

            location.SoftDelete();
            db.Entry(location).Property(l => l.DeletedAt).CurrentValue = DateTimeOffset.UtcNow - age;
            await db.SaveChangesAsync();
        });
    }

    private async Task BackdatePositionDeletion(Guid positionId, TimeSpan age)
    {
        await ExecuteInDb(async db =>
        {
            var position = await db.Positions
                .IgnoreQueryFilters()
                .FirstAsync(p => p.Id == new PositionId(positionId));

            position.SoftDelete();
            db.Entry(position).Property(p => p.DeletedAt).CurrentValue = DateTimeOffset.UtcNow - age;
            await db.SaveChangesAsync();
        });
    }
}