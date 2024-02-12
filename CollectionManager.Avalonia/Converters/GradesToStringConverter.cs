using Avalonia.Data.Converters;
using CollectionManager.Avalonia.Converters.Abstractions;
using CollectionManager.Core.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CollectionManager.Avalonia.Converters;

public sealed class GradesToStringConverter : MarkupExtensionImpl, IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        static string GradeToString(Grade value) => value switch
        {
            Grade.SSH or Grade.SS => "SS",
            Grade.SH or Grade.S => "S",
            Grade.A => "A",
            Grade.B => "B",
            Grade.C => "C",
            Grade.D => "D",
            _ => string.Empty,
        };

        foreach (Grade grade in values.OfType<Grade>())
        {
            return GradeToString(grade);
        }

        return string.Empty;
    }
}
