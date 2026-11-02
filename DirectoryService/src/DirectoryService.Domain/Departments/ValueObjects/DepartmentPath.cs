using CSharpFunctionalExtensions;
using Shared;
using SharedLibrary.SharedKernel;

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

    public Result<DepartmentPath, Errors> ReplaceLastSegment(DepartmentIdentifier identifier)
    {
        string[] path = Path.Split(SEPARATOR);
        path[^1] = identifier.Identifier;

        return CreatePath(string.Join(SEPARATOR, path));
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
}