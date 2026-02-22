using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.ValueObjects;

public sealed record DepartmentIndentifier
{
    private static readonly Regex Regex = new(@"^[A-Za-z]+$", RegexOptions.CultureInvariant);

    public string Value { get; }

    private DepartmentIndentifier(string value) => Value = value;

    public static Result<DepartmentIndentifier, string> Create(string value)
    {
        value = value.Trim();

        if (!StringValidator<DepartmentIndentifier>.For(value)
                .IsNullOrWhiteSpace()
                .LengthMinMax(4, 150)
                .StringFormat(Regex)
                .IsValid(out string errorMessage))
        {
            return errorMessage!;
        }


        return new DepartmentIndentifier(value);
    }
}