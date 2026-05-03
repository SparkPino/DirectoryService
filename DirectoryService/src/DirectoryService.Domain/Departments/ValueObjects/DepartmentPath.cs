using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentPath
{
    public string Path { get; }

    private const char SEPARATOR = '.';

    private DepartmentPath(string path) => Path = path;

    private static Result<DepartmentPath, Errors> CreatePath(string value)
    {
        value = value.Trim();

        if (!StringValidator<DepartmentPath>.For(value)
                .IsNullOrWhiteSpace()
                .StringFormat(AppRegexes.DepartmentPathRegex())
                .IsValid(out List<Error>? errorMessage))
        {
            return errorMessage!.First().ToErrors();
        }

        return new DepartmentPath(value);
    }

    public static Result<DepartmentPath, Errors> CreateParent(DepartmentIdentifier value)
    {
        return CreatePath(value.Identifier);
    }

    public Result<DepartmentPath, Errors> CreateChildPath(DepartmentIdentifier child)
    {
        if (!StringValidator<DepartmentPath>.For(child.Identifier)
                .IsNullOrWhiteSpace()
                .IsValid(out List<Error>? childErrorMessage))
        {
            return childErrorMessage!.First().ToErrors();
        }

        string childPath = $"{Path}{SEPARATOR}{child.Identifier}";

        return CreatePath(childPath);
    }

    public Result<DepartmentPath, Errors> RemoveChildFromPath(DepartmentIdentifier child)
    {
        if (!StringValidator<DepartmentPath>.For(child.Identifier)
                .IsNullOrWhiteSpace()
                .IsValid(out List<Error>? errorMessage))
            return errorMessage!.First().ToErrors();

        if (child.Identifier.Length <= 1)
            return Error.Validation("department.path.is.root", "Путь уже корневой").ToErrors();

        string[] path = Path.Split(SEPARATOR).ToArray();

        if (!path.Contains(child.Identifier))
        {
            return Error.NotFound(
                    "department.path.child.not.found",
                    $"Сегмент '{child.Identifier}'не найдено в пути")
                .ToErrors();
        }

        var newPath = path.Where(a => a != child.Identifier);

        string result = string.Join(SEPARATOR, newPath);

        return CreatePath(result);
    }
}