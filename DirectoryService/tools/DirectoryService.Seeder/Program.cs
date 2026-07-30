using System.Diagnostics;
using Bogus;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Seeder;
using Microsoft.EntityFrameworkCore;

Randomizer.Seed = new Random(12345);

var connectionString = Environment.GetEnvironmentVariable("SEEDER_DB_CONNECTION")
                       ?? throw new InvalidOperationException(
                           "SEEDER_DB_CONNECTION не задана. Явно укажи целевую БД перед запуском сидера — " +
                           "значение по умолчанию намеренно не предусмотрено, чтобы случайно не засеять не тот инстанс.");

Console.WriteLine($"Сидер пойдёт в:  {HidePassword.MaskPassword(connectionString)}");
Console.Write("Продолжить? (y/N): ");
if (Console.ReadLine()?.Trim().ToLowerInvariant() != "y")
{
    Console.WriteLine("Отменено.");
    return;
}

await using var context = new DirectoryServiceDbContext(
    new DbContextOptionsBuilder<DirectoryServiceDbContext>()
        .UseNpgsql(connectionString)
        .Options);

var dbCountElements = await context.Departments.IgnoreQueryFilters().CountAsync();
if (dbCountElements > 20)
{
    Console.WriteLine("В базе уже больше 20 департаментов, это точно свежая dev-БД?");
    return;
}

var faker = new Faker();

var departments = DepartmentTreeGenerator.Generate(faker);
Console.WriteLine($"Сгенерировано {departments.Count} департаментов в памяти (без обращений к БД).");

var locations = LocationGenerator.Generate(faker, count: 300);
Console.WriteLine($"Сгенерировано {locations.Count} локаций в памяти.");

var positions = PositionGenerator.Generate(faker, count: 250);
Console.WriteLine($"Сгенерировано {positions.Count} должностей в памяти.");

DepartmentRelationLinker.Link(faker, departments, locations, positions);
var totalRelations = departments.Sum(d => d.DepartmentsLocations.Count + d.DepartmentPositions.Count);
Console.WriteLine($"Создано {totalRelations} связей department_locations/department_positions.");

context.ChangeTracker.AutoDetectChangesEnabled = false;

var stopwatch = Stopwatch.StartNew();

Console.WriteLine("Записываю локации...");
await BatchPersister.SaveInBatchesAsync(context, locations, batchSize: 500, label: "Locations");

Console.WriteLine("Записываю должности...");
await BatchPersister.SaveInBatchesAsync(context, positions, batchSize: 500, label: "Positions");

// Дерево департаментов - это один связный объектный граф (ChildDepartments - настоящая EF-навигация),
// поэтому AddRange одного узла всё равно каскадно затягивает всё поддерево. Нарезка на батчи по размеру
// здесь не работает (EF игнорирует границы батча) - пишем всё дерево одним SaveChanges.
Console.WriteLine("Записываю департаменты (вместе со связями department_locations/department_positions)...");
await BatchPersister.SaveInBatchesAsync(context, departments, batchSize: departments.Count, label: "Departments");

stopwatch.Stop();
Console.WriteLine($"Сидинг завершён за {stopwatch.Elapsed:mm\\:ss}.");