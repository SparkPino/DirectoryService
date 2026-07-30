using Bogus;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;

namespace DirectoryService.Seeder;

public static class DepartmentRelationLinker
{
    public static void Link(
        Faker faker,
        List<Department> departments,
        List<Location> locations,
        List<Position> positions)
    {
        foreach (var department in departments)
        {
            var locationCount = faker.Random.Int(0, 3);
            var pickedLocationIds = faker.PickRandom(locations, locationCount).Select(l => l.Id).ToList();

            if (pickedLocationIds.Count > 0)
            {
                var addLocationsResult = department.AddLocations(pickedLocationIds);
                if (addLocationsResult.IsFailure)
                {
                    throw new InvalidOperationException(
                        $"Не удалось привязать локации к департаменту {department.Id.Id}: {addLocationsResult.Error.Message}");
                }
            }

            var positionCount = faker.Random.Int(0, 3);
            var pickedPositions = faker.PickRandom(positions, positionCount).ToList();

            foreach (var position in pickedPositions)
            {
                var addPositionResult = department.AddPosition(position.Id);
                if (addPositionResult.IsFailure)
                {
                    throw new InvalidOperationException(
                        $"Не удалось привязать должность к департаменту {department.Id.Id}: {addPositionResult.Error.Message}");
                }
            }
        }
    }
}