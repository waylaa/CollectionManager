using Avalonia.Data.Converters;
using CollectionManager.Avalonia.Converters.Abstractions;
using System;
using System.Globalization;

namespace CollectionManager.Avalonia.Converters;

public sealed class ByteOrFloatAsObjectToStringConverter : MarkupExtensionImpl, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => System.Convert.ToString(value, culture);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
