using CollectionManager.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CollectionManager.Avalonia.Comparers;

internal sealed class BeatmapTitleEqualityComparer : IEqualityComparer<OsuBeatmap>
{
    public bool Equals(OsuBeatmap? x, OsuBeatmap? y)
        => x is not null && y is not null && x.Title == y.Title;

    public int GetHashCode([DisallowNull] OsuBeatmap obj)
        => obj.GetHashCode();
}
