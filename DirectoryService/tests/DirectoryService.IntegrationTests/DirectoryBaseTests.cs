using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Departments.Commands.AddDepartment;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Domain.Shared.ValueObjects;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Infrastructure.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace DirectoryService.IntegrationTests;

[CollectionDefinition(Name)]
public class DirectoryServiceCollection : ICollectionFixture<DirectoryServiceWebFactory>
{
    public const string Name = "DirectoryService collection";
}

[Collection(DirectoryServiceCollection.Name)]
public class DirectoryBaseTests : IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;

    protected IServiceProvider Services { get; set; }

    protected DirectoryBaseTests(DirectoryServiceWebFactory factory)
    {
        Services = factory.Services;
        _resetDatabase = factory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();

    protected async Task<Result<TResult, Errors>> ExecuteHandler<TCommand, TResult>(
        Func<ICommandHandler<TCommand, TResult>, Task<Result<TResult, Errors>>> action)
        where TCommand : ICommand
    {
        await using var scope = Services.CreateAsyncScope();

        // sut - system under test регламентированное название
        var sut = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();

        return await action(sut);
    }

    protected async Task<Result<TResult, Errors>> ExecuteQueryHandler<TQuery, TResult>(
        Func<IQueryHandler<TQuery, TResult>, Task<Result<TResult, Errors>>> action)
        where TQuery : IQuery
    {
        await using var scope = Services.CreateAsyncScope();

        var sut = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

        return await action(sut);
    }

    protected async Task<TResult> ExecuteRepository<TRepository, TResult>(
        Func<TRepository, Task<TResult>> action)
        where TRepository : notnull
    {
        await using var scope = Services.CreateAsyncScope();

        var repository = scope.ServiceProvider.GetRequiredService<TRepository>();

        return await action(repository);
    }

    protected async Task<T> ExecuteInDb<T>(Func<DirectoryServiceDbContext, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();

        var dbcontext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        return await action(dbcontext);
    }

    protected async Task ExecuteInDb(Func<DirectoryServiceDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();

        var dbcontext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        await action(dbcontext);
    }

    protected async Task ExecuteInReadDbContext(Func<IReadDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var readDbContext = scope.ServiceProvider.GetRequiredService<IReadDbContext>();
        await action(readDbContext);
    }

    protected async Task<Guid> CreateLocationAsync(string name)
    {
        var locationNameResult = LocationName.Create(name).Value;

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

    protected async Task<Guid> CreateDepartmentAsync(string name, string identifier, Guid? parentId = null)
    {
        return await ExecuteInDb(async db =>
        {
            Department? parent = null;
            if (parentId.HasValue)
            {
                parent = await db.Departments.FirstAsync(d => d.Id == new DepartmentId(parentId.Value));
            }

            var department = Department.CreateDepartment(
                [],
                DepartmentName.Create(name).Value,
                DepartmentIdentifier.Create(identifier).Value,
                parent);


            await db.Departments.AddAsync(department.Value);
            await db.SaveChangesAsync();

            return department.Value.Id.Id;
        });
    }

    protected async Task<(Guid alpha, Guid beta, Guid gamma, Guid delta, Guid epsilon)> SeedAlphaHierarchyAsync()
    {
        var alphaId = await CreateDepartmentAsync("Alpha", "alpha");
        var betaId = await CreateDepartmentAsync("Beta", "beta", alphaId);
        var gammaId = await CreateDepartmentAsync("Gamma", "gamma", betaId);
        var deltaId = await CreateDepartmentAsync("Delta", "delta", betaId);
        var epsilonId = await CreateDepartmentAsync("Epsilon", "epsilon", alphaId);

        return (alphaId, betaId, gammaId, deltaId, epsilonId);
    }

    protected async Task<Guid> CreatePositionAsync(string name, string? description = null)
    {
        var position = Position.Create(PositionName.Create(name).Value, description).Value;

        return await ExecuteInDb(async db =>
        {
            await db.Positions.AddAsync(position);
            await db.SaveChangesAsync();
            return position.Id.Id;
        });
    }
}