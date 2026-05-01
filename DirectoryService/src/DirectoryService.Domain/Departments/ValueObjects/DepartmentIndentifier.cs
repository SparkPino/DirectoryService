using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentIndentifier
{
    private static readonly Regex Regex = new(@"^[A-Za-z]+$", RegexOptions.CultureInvariant);

    public string Identifier { get; }

    private DepartmentIndentifier(string identifier) => Identifier = identifier;

    public static Result<DepartmentIndentifier, Errors> Create(string value)
    {
        value = value.Trim();

        if (!StringValidator<DepartmentIndentifier>.For(value)
                .IsNullOrWhiteSpace()
                .LengthMinMax(4, 150)
                .StringFormat(Regex)
                .IsValid(out List<Error> errorMessage))
        {
            return new Errors(errorMessage);
        }


        return new DepartmentIndentifier(value);
    }

    public static DepartmentIndentifier FromDb(string value)
    {
        return new DepartmentIndentifier(value);
    }
}