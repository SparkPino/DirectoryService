using System.Text;

namespace DirectoryService.Domain.Shared;

public record StringError
{
    public string Message { get; private set; }

    private StringBuilder builder = new();

    public StringError()
    {
    }

    public void AddErrorMessage(string message)
    {
        builder.AppendLine(message);
    }

    public string GetAllErrorMessage()
    {
        return builder.ToString();
    }
}

public enum ErrorType
{
    VALIDATION,
    NOT_FOUND,
    FAILURE,
    CONFLICT,
    AUTHENTICATION,
    AUTHORIZATION,
}