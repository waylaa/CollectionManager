using CollectionManager.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.ObjectModel;

namespace CollectionManager.Avalonia.Extensions;

internal static class MemoryCacheExtensions
{
    /// <summary>
    /// Gets all the currently cached .osdb collections.
    /// </summary>
    /// <param name="cache">The in-memory cache.</param>
    /// <returns>[<see cref="ReadOnlyCollection{T}"/>] A read-only collection of cached .osdb collections or an empty read-only collection
    /// if no .osdb collection has been loaded.</returns>
    internal static ReadOnlyCollection<OsdbCollection> GetAllCollections(this IMemoryCache cache)
        => cache.Get<ReadOnlyCollection<OsdbCollection>>(MemoryCacheKeys.Collections) ?? ReadOnlyCollection<OsdbCollection>.Empty;

    /// <summary>
    /// Gets all the currently cached beatmaps.
    /// </summary>
    /// <param name="cache">The in-memory cache.</param>
    /// <returns>[<see cref="ReadOnlyCollection{T}"/>] All the beatmaps stored in the 'Songs' folder of the osu! game folder
    /// or an empty read-only collection if the osu! database is not loaded.</returns>
    internal static ReadOnlyCollection<OsuBeatmap> GetAllBeatmaps(this IMemoryCache cache)
        => cache.Get<ReadOnlyCollection<OsuBeatmap>>(MemoryCacheKeys.Beatmaps) ?? ReadOnlyCollection<OsuBeatmap>.Empty;

    internal static void AddCollections(this IMemoryCache cache, ReadOnlyCollection<OsdbCollection> newCollections)
        => cache.Set(MemoryCacheKeys.Collections, newCollections);

    internal static void AddBeatmaps(this IMemoryCache cache, ReadOnlyCollection<OsuBeatmap> newBeatmaps)
        => cache.Set(MemoryCacheKeys.Beatmaps, newBeatmaps);

    internal static void AddDatabaseFilePath(this IMemoryCache cache, string path)
        => cache.Set(MemoryCacheKeys.DatabaseFilePath, path);

    internal static void UnloadDatabase(this IMemoryCache cache)
    {
        cache.Remove(MemoryCacheKeys.DatabaseFilePath);
        cache.Remove(MemoryCacheKeys.Beatmaps);
    }

    /*
    internal static bool IsCachedPropertyEqual<TCached, TEnumerable, TItem>(
        this IMemoryCache cache,
        string key,
        Func<TCached, TEnumerable> propertySelector,
        TEnumerable value)
        where TEnumerable : IReadOnlyList<TItem>
        => cache.TryGetValue(key, out TCached? cachedValue) && cachedValue is not null && propertySelector(cachedValue).SequenceEqual(value);
    */
}

internal sealed class MemoryCacheKeys
{
    internal static string DatabaseFilePath => "database-filepath";

    internal static string Beatmaps => "all-beatmaps";

    internal static string Collections => "all-collections";
}
