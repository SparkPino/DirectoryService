using System.Text.RegularExpressions;

namespace Shared;

public static partial class AppRegexes
{
    [GeneratedRegex(@"^[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*$", RegexOptions.CultureInvariant)]
    public static partial Regex DepartmentPathRegex();

    [GeneratedRegex(@"^[A-Za-z]+$", RegexOptions.CultureInvariant)]
    public static partial Regex DepartmentIndentifierRegex();
}