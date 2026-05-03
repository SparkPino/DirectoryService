using System.Text.RegularExpressions;

namespace Shared;

public sealed class StringValidator<TValue>
{
    private readonly string? _value;

    private bool _isValid = true;

    // private StringError _stringError;
    private readonly List<Error> _stringError;

    private readonly string? _invalidFieldName;

    private StringValidator(string? value, string? invalidFieldName = null)
    {
        _stringError = new();
        _value = value;
        _invalidFieldName = invalidFieldName;
    }

    public static StringValidator<TValue> For(string? value)
        => new StringValidator<TValue>(value);

    public static StringValidator<TValue> For(string? value, string invalidFieldName)
        => new StringValidator<TValue>(value, invalidFieldName);

    public StringValidator<TValue> IsNullOrWhiteSpace()
    {
        if (string.IsNullOrWhiteSpace(_value))
        {
            _stringError.Add(Error.Validation(
                $"{_invalidFieldName ?? typeof(TValue).Name}.is.null.or.whitespace",
                $"{_invalidFieldName ?? typeof(TValue).Name} не может быть пустым или состоять из пробелов",
                _invalidFieldName));
            _isValid = false;
        }

        return this;
    }


    public StringValidator<TValue> LengthMinMax(int min, int max)
    {
        if (_value.Length < min || _value.Length > max)
        {
            _stringError.Add(Error.Validation(
                $"{_invalidFieldName ?? typeof(TValue).Name}.length.min.max",
                $"{_invalidFieldName ?? typeof(TValue).Name} не может быть меньше {min} и больше {max} символов",
                _invalidFieldName));
            _isValid = false;
        }

        return this;
    }

    public StringValidator<TValue> StringFormat(Regex regex, string? message = null)
    {
        if (!regex.IsMatch(_value))
        {
            if (message != null)
            {
                _stringError.Add(Error.Validation(
                    $"{_invalidFieldName ?? typeof(TValue).Name}.string.format",
                    $"{_invalidFieldName ?? typeof(TValue).Name} {message}",
                    _invalidFieldName));
            }
            else
            {
                _stringError.Add(Error.Validation(
                    $"{_invalidFieldName ?? typeof(TValue).Name}.string.format",
                    $"{_invalidFieldName ?? typeof(TValue).Name} должен содержать только латинские буквы",
                    _invalidFieldName));
            }

            _isValid = false;
        }

        return this;
    }


    public bool IsValid(out List<Error>? errorMessages)
    {
        if (_isValid)
        {
            errorMessages = null;
            return true;
        }

        errorMessages = _stringError;

        return false;
    }


    public bool IsValid()
    {
        return _isValid;
    }
}