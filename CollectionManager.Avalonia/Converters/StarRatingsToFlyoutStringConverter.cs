using Avalonia.Data.Converters;
using CollectionManager.Avalonia.Converters.Abstractions;
using CollectionManager.Core.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CollectionManager.Avalonia.Converters;

public sealed class StarRatingsToFlyoutStringConverter : MarkupExtensionImpl, IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        StringBuilder builder = new($"Star ratings for every possible mod combination: {Environment.NewLine}");
        KeyValuePair<Ruleset, ReadOnlyDictionary<Mods, double>> modCombinationList = values.OfType<KeyValuePair<Ruleset, ReadOnlyDictionary<Mods, double>>>().FirstOrDefault();

        if (modCombinationList.Value is null)
        {
            return string.Empty;
        }

        IEnumerable<string> modCombinations = modCombinationList.Value
            .Select(starRatings =>
            {
                string modCombination = starRatings.Key switch
                {
                    Mods.Nomod => "NM",
                    Mods.Easy => "EZ",
                    Mods.HalfTime => "HT",
                    Mods.HardRock => "HR",
                    Mods.DoubleTime or Mods.Nightcore => "DT/NC",
                    Mods.Flashlight => "FL",
                    Mods.Easy | Mods.HalfTime => "EZHT",
                    (Mods.Easy | Mods.DoubleTime) or (Mods.Easy | Mods.Nightcore) => "EZDT/EZNC",
                    Mods.Easy | Mods.Flashlight => "EZFL",
                    Mods.HalfTime | Mods.Flashlight => "HTFL",
                    Mods.HalfTime | Mods.HardRock => "HTHR",
                    (Mods.HardRock | Mods.DoubleTime) or (Mods.HardRock | Mods.Nightcore) => "HRDT/HRNC",
                    Mods.HardRock | Mods.Flashlight => "HRFL",
                    (Mods.DoubleTime | Mods.Flashlight) or (Mods.Nightcore | Mods.Flashlight) => "DTFL/NCFL",
                    (Mods.Easy | Mods.DoubleTime | Mods.Flashlight) or (Mods.Easy | Mods.Nightcore | Mods.Flashlight) => "EZDTFL/EZNCFL",
                    (Mods.HardRock | Mods.DoubleTime | Mods.Flashlight) or (Mods.HardRock | Mods.Nightcore | Mods.Flashlight) => "HRDTFL/HRNCFL",
                    _ => string.Empty
                };

                return (modCombination, starRatings.Value);
            })
            .Where(representableModCombination => !string.IsNullOrEmpty(representableModCombination.modCombination))
            .Select(result => $"{result.modCombination} - {result.Value} ★");

        return builder
            .AppendLine(string.Join(Environment.NewLine, modCombinations))
            .ToString()
            .TrimEnd();
    }
}
