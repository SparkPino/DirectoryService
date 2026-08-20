using DirectoryService.Infrastructure.Postgres;

namespace DirectoryService.Seeder;

public static class BatchPersister
{
    public static async Task SaveInBatchesAsync<T>(
        DirectoryServiceDbContext context,
        List<T> items,
        int batchSize,
        string label)
        where T : class
    {
        for (var offset = 0; offset < items.Count; offset += batchSize)
        {
            var count = Math.Min(batchSize, items.Count - offset);
            var batch = items.GetRange(offset, count);

            context.AddRange(batch);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            Console.WriteLine($"  {label}: {Math.Min(offset + batchSize, items.Count)}/{items.Count}");
        }
    }
}