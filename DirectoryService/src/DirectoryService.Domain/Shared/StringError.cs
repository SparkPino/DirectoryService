using System.Text;

namespace DirectoryService.Domain.Shared;

public record StringError
{
    public string Message { get; private set; }

    private StringBuilder builder = new();

    public StringError()
    {
    }

    public StringError AddErrorMessage(params string[] messages)
    {
        foreach (var message in messages)
        {
            builder.AppendLine(message);
        }

        return this;
    }

    public string GetAllErrorMessage()
    {
        return builder.ToString();
    }
}