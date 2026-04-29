using System.Text.Json.Serialization;

namespace Shared;

public class Envelope
{
    public object? Result { get; }

    public Errors? ErrorList { get; }

    public bool IsError => ErrorList != null && ErrorList.Any();

    public DateTime TimeGenerated { get; }

    [JsonConstructor]
    private Envelope(object? result, Errors? errorList)
    {
        Result = result;
        ErrorList = errorList;
        TimeGenerated = DateTime.UtcNow;
    }

    public static Envelope Ok(object? result = null) => new(result, null);

    public static Envelope Error(Errors errors) => new(null, errors);
}

public class Envelope<TValue>
{
    public TValue Result { get; }

    public Errors? ErrorList { get; }

    public bool IsError => ErrorList != null && ErrorList.Any();

    public DateTime TimeGenerated { get; }

    [JsonConstructor]
    private Envelope(TValue? result, Errors? errorList)
    {
        Result = result;
        ErrorList = errorList;
        TimeGenerated = DateTime.UtcNow;
    }

    public static Envelope<TValue> Ok(TValue? result = default) => new(result, null);

    public static Envelope<TValue> Error(Errors errors) => new(default, errors);
}