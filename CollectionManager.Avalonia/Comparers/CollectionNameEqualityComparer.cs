using CollectionManager.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CollectionManager.Avalonia.Comparers;

internal sealed class CollectionNameEqualityComparer : IEqualityComparer<OsdbCollection>
{
    public bool Equals(OsdbCollection? x, OsdbCollection? y)
        => x is not null && y is not null && x.Name == y.Name;

    public int GetHashCode([DisallowNull] OsdbCollection obj)
        => obj.GetHashCode();
}
