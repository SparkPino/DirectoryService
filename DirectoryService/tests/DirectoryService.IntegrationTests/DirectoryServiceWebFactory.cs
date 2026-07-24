using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Infrastructure.Postgres.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Program = DirectoryService.Presentation.Program;

namespace DirectoryService.IntegrationTests;

public class DirectoryServiceWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("directory_service_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private Respawner _respawner = null!;
    private NpgsqlConnection _dbConnection = null!;


    protected override void ConfigureWebHost(IWebHostBuilder builder) // работа с di services
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DirectoryServiceDbContext>();

            services.AddDbContextPool<DirectoryServiceDbContext>((sp, options) =>
            {
                string? connectionString = _container.GetConnectionString();

                IHostEnvironment hostEnvironment = sp.GetRequiredService<IHostEnvironment>(); // дает состояние проекта

                ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>(); // берем из di


                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
                    npgsqlOptions.MigrationsAssembly(typeof(DirectoryServiceDbContext).Assembly.FullName);
                });

                if (hostEnvironment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }

                options.UseLoggerFactory(loggerFactory);
            });

            //QueryContext
            services.AddScoped<IReadDbContext>(sp =>
                sp.GetRequiredService<DirectoryServiceDbContext>()); //Берем уже созданное подключение,не дублируем

            //Dapper
            services.RemoveAll<IDbConnectionFactory>();
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgsqlDbConnectionFactory(_container.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        await dbContext.Database.EnsureDeletedAsync(); //Что бы проверить что базы нету (она удалена)
        await dbContext.Database.EnsureCreatedAsync(); //Создаем базу для контекста если ее нету

        _dbConnection = new NpgsqlConnection(_container.GetConnectionString());
        await _dbConnection.OpenAsync();

        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _dbConnection.CloseAsync();
        await _dbConnection.DisposeAsync();

        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    private async Task InitializeRespawner()
    {
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            options: new RespawnerOptions { DbAdapter = DbAdapter.Postgres, SchemasToInclude = new[] { "public" }, });
    }
}