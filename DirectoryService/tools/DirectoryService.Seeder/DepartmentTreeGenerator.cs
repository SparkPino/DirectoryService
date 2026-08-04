using System.Text;
using Bogus;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;

namespace DirectoryService.Seeder;

public static class DepartmentTreeGenerator
{
    private const int ROOT_COUNT = 8;
    private static readonly int[] _branchingFactors = [14, 11, 7];

    public static List<Department> Generate(Faker faker)
    {
        var identifierCounter = 0;

        Department CreateDepartmentOrThrow(Department? parent)
        {
            identifierCounter++;
            var identifier = $"dept{ToLetterSuffix(identifierCounter)}";
            var name = faker.Commerce.Department();

            var departmentResult = Department.CreateDepartment(
                [],
                DepartmentName.Create(name).Value,
                DepartmentIdentifier.Create(identifier).Value,
                parent);

            if (departmentResult.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Не удалось создать департамент '{name}' ({identifier}): " +
                    string.Join(", ", departmentResult.Error.Select(e => e.Message)));
            }

            return departmentResult.Value;
        }

        var allDepartments = new List<Department>();
        var currentLevel = new List<Department>();

        for (var i = 0; i < ROOT_COUNT; i++)
        {
            var department = CreateDepartmentOrThrow(parent: null);
            currentLevel.Add(department);
            allDepartments.Add(department);
        }

        foreach (var childrenPerParent in _branchingFactors)
        {
            var nextLevel = new List<Department>();

            foreach (var parent in currentLevel)
            {
                for (var i = 0; i < childrenPerParent; i++)
                {
                    var child = CreateDepartmentOrThrow(parent);
                    nextLevel.Add(child);
                    allDepartments.Add(child);
                }
            }

            currentLevel = nextLevel;
        }

        return allDepartments;
    }

    private static string ToLetterSuffix(int number)
    {
        var sb = new StringBuilder();
        while (number > 0)
        {
            number--;
            sb.Insert(0, (char)('a' + number % 26));
            number /= 26;
        }

        return sb.ToString();
    }
}