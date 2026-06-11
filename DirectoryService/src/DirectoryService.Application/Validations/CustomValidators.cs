using System.Text.Json;
using CSharpFunctionalExtensions;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Validations;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Error>> factoryMethod)
    {
        return ruleBuilder.Custom((model, context) =>
        {
            Result<TValueObject, Error> result = factoryMethod.Invoke(model);
            if (result.IsSuccess) return;

            context.AddFailure(JsonSerializer.Serialize(result.Error));
        });
    }

    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Errors>> factoryMethod)
    {
        return ruleBuilder.Custom((model, context) =>
        {
            Result<TValueObject, Errors> result = factoryMethod.Invoke(model);
            if (result.IsSuccess) return;

            foreach (var error in result.Error)
            {
                context.AddFailure(JsonSerializer.Serialize(error));
            }
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> ruleBuilder,
        Error error) => ruleBuilder.WithMessage(JsonSerializer.Serialize(error));
}