using Avalonia.Data.Converters;
using CollectionManager.Avalonia.Converters.Abstractions;
using CollectionManager.Core.Objects;
using System;
using System.Globalization;

namespace CollectionManager.Avalonia.Converters;

public sealed class StatusToStringConverter : MarkupExtensionImpl, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Status status)
        {
            return string.Empty;
        }

        return status switch
        {
            Status.Unknown => "Unknown",
            Status.Unsubmitted => "Unsumbmitted",
            Status.Pending => "Pending",
            Status.Unused => "Unused",
            Status.Ranked => "Ranked",
            Status.Approved => "Approved",
            Status.Qualified => "Qualified",
            Status.Loved => "Loved",
            _ => string.Empty
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
