using Avalonia.Data.Converters;
using CollectionManager.Avalonia.Converters.Abstractions;
using System;
using System.Globalization;

namespace CollectionManager.Avalonia.Converters;

public sealed class BpmToStringConverter : MarkupExtensionImpl, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not (double minBpm, double maxBpm))
        {
            return string.Empty;
        }

        return minBpm == maxBpm ? minBpm.ToString() : $"{minBpm} - {maxBpm}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
