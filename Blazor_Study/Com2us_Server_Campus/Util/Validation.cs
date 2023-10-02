using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIServer.Util;

public class TimeSpanRangeAttribute : ValidationAttribute
{
    private readonly TimeSpan _minValue;
    private readonly TimeSpan _maxValue;

    public TimeSpanRangeAttribute(string minValue, string maxValue)
    {
        _minValue = TimeSpan.Parse(minValue);
        _maxValue = TimeSpan.Parse(maxValue);
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is TimeSpan timeSpanValue)
        {
            if (timeSpanValue < _minValue || timeSpanValue > _maxValue)
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}