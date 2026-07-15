namespace DirectoryService.Infrastructure.BackgroundServices;

public class SoftDeleteCleanupOptions
{
    public int RetentionDays { get; init; }

    public int BatchSize { get; init; }

    public int IntervalMinutes { get; init; }
}