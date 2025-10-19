using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CubeCell.App.Converters;

public class NullToBoolConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = value != null;
        return Invert ? !result : result;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}