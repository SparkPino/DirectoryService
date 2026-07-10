using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Departments.Commands.AddDepartment;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests;

public class DirectoryBaseTests : IClassFixture<DirectoryServiceWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _reserDatabase;

    protected IServiceProvider Services { get; set; }

    public DirectoryBaseTests(DirectoryServiceWebFactory factory)
    {
        Services = factory.Services;
        _reserDatabase = factory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _reserDatabase();

    protected async Task<T> ExecuteHandler<T>(Func<ICommandHandler<AddDepartmentCommand, Guid>, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();

        // sut - system under test регламентированное название
        var sut = scope.ServiceProvider.GetRequiredService<ICommandHandler<AddDepartmentCommand, Guid>>();

        return await action(sut);
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
}