using Avalonia.Data.Converters;
using CollectionManager.Avalonia.Converters.Abstractions;
using System;
using System.Globalization;

namespace CollectionManager.Avalonia.Converters;

public sealed class DateTimeToStringConverter : MarkupExtensionImpl, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime dateTime || dateTime == DateTime.MinValue)
        {
            return string.Empty;
        }

        return dateTime.ToShortDateString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
