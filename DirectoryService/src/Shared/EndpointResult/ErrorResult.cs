using Microsoft.AspNetCore.Http;

namespace Shared.EndpointResult;

public class ErrorResult : IResult
{
    private readonly Errors _errors;

    public ErrorResult(Error error)
    {
        _errors = error.ToErrors();
    }

    public ErrorResult(Errors errors)
    {
        _errors = errors;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (!_errors.Any())
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return httpContext.Response.WriteAsJsonAsync(Envelope.Error(_errors));
        }

        var distinctErrorType = _errors
            .Select(error => error.Type)
            .Distinct()
            .ToList();

        int statusCode = distinctErrorType.Count > 1
            ? StatusCodes.Status500InternalServerError
            : GetStatusCodeForErrorType(distinctErrorType.First());

        var envelope = Envelope.Error(_errors);

        httpContext.Response.StatusCode = statusCode;
        return httpContext.Response.WriteAsJsonAsync(envelope);
    }

    private static int GetStatusCodeForErrorType(ErrorType type)
    {
        int statusCode = type switch
        {
            ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
            ErrorType.AUTHENTICATION => StatusCodes.Status401Unauthorized,
            ErrorType.AUTHORIZATION => StatusCodes.Status403Forbidden,
            ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ErrorType.CONFLICT => StatusCodes.Status409Conflict,
            ErrorType.FAILURE => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,
        };
        return statusCode;
    }
}