using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.ValueObjects;

public sealed record DepartmentPath
{
    private static readonly Regex _regex = new Regex(@"^[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*$",
        RegexOptions.CultureInvariant);

    public string Value { get; }

    private DepartmentPath(string value) => Value = value;

    public static Result<DepartmentPath, string> Create(string value)
    {
        value = value.Trim();

        if (!StringValidator<DepartmentPath>.For(value).IsNullOrWhiteSpace().StringFormat(_regex)
                .IsValid(out string? errorMessage))
        {
            return errorMessage;
        }

        return new DepartmentPath(value);
    }

    public static Result<DepartmentPath, string> AddChildToPath(string parentPath, string child)
    {
        var errors = new StringError();

        var parentValid = StringValidator<DepartmentPath>.For(parentPath)
            .IsNullOrWhiteSpace()
            .StringFormat(_regex)
            .IsValid(out var parentErrorMessage);

        var childValid = StringValidator<DepartmentPath>.For(child)
            .IsNullOrWhiteSpace()
            .IsValid(out var childErrorMessage);

        if (!parentValid || !childValid)
        {
            if (parentErrorMessage != null) errors.AddErrorMessage(parentErrorMessage);
            if (childErrorMessage != null) errors.AddErrorMessage(childErrorMessage);

            return errors.GetAllErrorMessage();
        }

        var childPath = $"{parentPath}.{child}";


        return Create(childPath);
    }

    public static Result<DepartmentPath, string> RemoveChildFromPath(string path)
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
}