using Avalonia.Data.Converters;
using CollectionManager.Avalonia.Converters.Abstractions;
using CollectionManager.Core.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace CollectionManager.Avalonia.Converters;

public sealed class StarRatingsToNomodStarRatingAsStringConverter : MarkupExtensionImpl, IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        foreach (KeyValuePair<Ruleset, ReadOnlyDictionary<Mods, double>> starRating in values.OfType<KeyValuePair<Ruleset, ReadOnlyDictionary<Mods, double>>>())
        {
            if (starRating.Value is null or { Count: 0 })
            {
                continue;
            }

            return starRating.Value[Mods.Nomod].ToString();
        }

        return string.Empty;
    }
}
