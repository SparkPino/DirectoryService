using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentPath
{
    private static readonly Regex _regex = new Regex(@"^[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*$",
        RegexOptions.CultureInvariant);

    public string Path { get; }

    private DepartmentPath(string path) => Path = path;

    public static Result<DepartmentPath, Errors> Create(string value)
    {
        value = value.Trim();

        if (!StringValidator<DepartmentPath>.For(value).IsNullOrWhiteSpace().StringFormat(_regex)
                .IsValid(out List<Error> errorMessage))
        {
            return new Errors(errorMessage);
        }

        return new DepartmentPath(value);
    }

    public static Result<DepartmentPath, Errors> AddChildToPath(string parentPath, string child)
    {
        var parentValid = StringValidator<DepartmentPath>.For(parentPath)
            .IsNullOrWhiteSpace()
            .StringFormat(_regex)
            .IsValid(out List<Error> parentErrorMessage);

        var childValid = StringValidator<DepartmentPath>.For(child)
            .IsNullOrWhiteSpace()
            .IsValid(out List<Error> childErrorMessage);

        if (!parentValid || !childValid)
        {
            return new Errors([..parentErrorMessage ?? [], ..childErrorMessage ?? []]);
        }

        var childPath = $"{parentPath}.{child}";


        return Create(childPath);
    }

    public static Result<DepartmentPath, Errors> RemoveChildFromPath(string path)
    {
        string[] newValue = new string[path.Length - 1];
        var value = path.Split('.');
        for (int i = 0; i < value.Length - 1; i++)
        {
            newValue[i] = value[i];
        }

        var result = string.Join(".", newValue);
        return Create(result);
    }

    public static DepartmentPath FromDb(string value)
    {
        return new DepartmentPath(value);
    }
}