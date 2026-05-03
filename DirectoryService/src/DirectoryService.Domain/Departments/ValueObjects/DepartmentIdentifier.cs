using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentIdentifier
{
    public const int IDENTIFIER_MIN_LENGTH = 4;
    public const int IDENTIFIER_MAX_LENGTH = 150;

    public string Identifier { get; }

    private DepartmentIdentifier(string identifier) => Identifier = identifier;

    public static Result<DepartmentIdentifier, Errors> Create(string value)
    {
        value = value.Trim();

        if (!StringValidator<DepartmentIdentifier>.For(value)
                .IsNullOrWhiteSpace()
                .LengthMinMax(IDENTIFIER_MIN_LENGTH, IDENTIFIER_MAX_LENGTH)
                .StringFormat(AppRegexes.DepartmentIndentifierRegex())
                .IsValid(out List<Error>? errorMessage))
        {
            return new Errors(errorMessage!);
        }

        return new DepartmentIdentifier(value);
    }

    public static DepartmentIdentifier FromDb(string value)
    {
        return new DepartmentIdentifier(value);
    }
}