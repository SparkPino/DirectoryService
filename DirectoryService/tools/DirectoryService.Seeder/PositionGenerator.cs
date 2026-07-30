using Bogus;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;

namespace DirectoryService.Seeder;

public static class PositionGenerator
{
    public static List<Position> Generate(Faker faker, int count)
    {
        var positions = new List<Position>(count);

        for (var i = 0; i < count; i++)
        {
            var name = faker.Name.JobTitle();
            var description = faker.Random.Bool(0.7f) ? faker.Lorem.Sentence() : null;

            var nameResult = PositionName.Create(name);
            if (nameResult.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Не удалось создать название должности '{name}': " +
                    string.Join(", ", nameResult.Error.Select(e => e.Message)));
            }

            var positionResult = Position.Create(nameResult.Value, description);
            if (positionResult.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Не удалось создать должность '{name}': {positionResult.Error.Message}");
            }

            positions.Add(positionResult.Value);
        }

        return positions;
    }
}