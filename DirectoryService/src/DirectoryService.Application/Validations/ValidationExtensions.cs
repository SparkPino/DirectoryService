using System.Text.Json;
using FluentValidation.Results;
using Shared;

namespace DirectoryService.Application.Validations;

public static class ValidationExtensions
{
    public static Errors ToError(this ValidationResult validationResult)
    {
        List<ValidationFailure> validationFailures = validationResult.Errors;
        var errors =
            from validationFailure in validationFailures
            let errorMessage = validationFailure.ErrorMessage
            let error = JsonSerializer.Deserialize<Error>(errorMessage)
            where error != null
            select error;

        return errors.ToList();
    }

    public static Errors ToError(this IEnumerable<ValidationResult> validationResult) =>
        validationResult.SelectMany(result => result.ToError()).ToList();
}