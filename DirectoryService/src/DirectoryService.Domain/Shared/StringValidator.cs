using System.Text.RegularExpressions;

namespace DirectoryService.Domain.Shared;

public sealed class StringValidator<T>
{
    private readonly string? _value;
    private bool _isValid = true;
    private StringError _stringError;

    private StringValidator(string? value)
    {
        _stringError = new();
        _value = value;
    }

    public static StringValidator<T> For(string? value)
        => new StringValidator<T>(value);

    public StringValidator<T> IsNullOrWhiteSpace()
    {
        if (string.IsNullOrWhiteSpace(_value))
        {
            _stringError.AddErrorMessage($"{typeof(T).Name} не может быть пустым или состоять из пробелов");
            _isValid = false;
        }

        return this;
    }


    public StringValidator<T> LengthMinMax(int min, int max)
    {
        if (_value.Length < min || _value.Length > max)
        {
            _stringError.AddErrorMessage(
                $"{typeof(T).Name} не может быть меньше {min} символов и больше {max} символов");
            _isValid = false;
        }

        return this;
    }

    public StringValidator<T> StringFormat(Regex regex, string? message = null)
    {
        if (!regex.IsMatch(_value))
        {
            if (message != null)
            {
                _stringError.AddErrorMessage($"{typeof(T).Name} {message}");
            }
            else
            {
                _stringError.AddErrorMessage($"{typeof(T).Name} должен содержать только латинские буквы");
            }

            _isValid = false;
        }

        return this;
    }

    public bool IsValid(out string? ErrorMessage)
    {
        if (_isValid)
        {
            ErrorMessage = null;
            return true;
        }

        ErrorMessage = _stringError.GetAllErrorMessage();

        return false;
    }

    public bool IsValid()
    {
        return _isValid;
    }


    public StringError GetError() => _stringError;
}