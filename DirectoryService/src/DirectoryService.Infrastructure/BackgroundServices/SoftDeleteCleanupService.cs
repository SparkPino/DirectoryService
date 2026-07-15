using DirectoryService.Application.Abstraction.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DirectoryService.Infrastructure.BackgroundServices;

public class SoftDeleteCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SoftDeleteCleanupService> _logger;
    private readonly IOptionsMonitor<SoftDeleteCleanupOptions> _optionsMonitor;

    public SoftDeleteCleanupService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SoftDeleteCleanupService> logger,
        IOptionsMonitor<SoftDeleteCleanupOptions> optionsMonitor)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _optionsMonitor = optionsMonitor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            int deleteCount = 0;

            do
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ISoftDeleteCleanupRepository>();

                var time = DateTime.UtcNow;
                var toDelete = time.AddDays(-_optionsMonitor.CurrentValue.RetentionDays);
                int batchSize = _optionsMonitor.CurrentValue.BatchSize;

                var departmentCountResult =
                    await db.DeleteExpiredDepartmentsBatchAsync(toDelete, batchSize, stoppingToken);
                var locationCountResult = await db.DeleteExpiredLocationsBatchAsync(toDelete, batchSize, stoppingToken);
                var positionCountResult = await db.DeleteExpiredPositionsBatchAsync(toDelete, batchSize, stoppingToken);

                if (departmentCountResult.IsFailure || locationCountResult.IsFailure || positionCountResult.IsFailure)
                {
                    _logger.LogError("Прогон очистки завершился с ошибкой, ждём следующего интервала");
                    break;
                }

                deleteCount = departmentCountResult.Value + locationCountResult.Value + positionCountResult.Value;
            } while (deleteCount > 0);

            await Task.Delay(TimeSpan.FromMinutes(_optionsMonitor.CurrentValue.IntervalMinutes), stoppingToken);
        }
    }
}