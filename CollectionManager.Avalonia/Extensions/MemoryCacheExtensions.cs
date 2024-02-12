using CollectionManager.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CollectionManager.Avalonia.Extensions;

internal static class MemoryCacheExtensions
{
    internal static ReadOnlyCollection<OsdbCollection> GetAllCollections(this IMemoryCache cache)
        => cache.Get<ReadOnlyCollection<OsdbCollection>>(MemoryCacheKeys.Collections)!;

    internal static ReadOnlyCollection<OsuBeatmap> GetAllBeatmaps(this IMemoryCache cache)
        => cache.Get<ReadOnlyCollection<OsuBeatmap>>(MemoryCacheKeys.Beatmaps)!;

    internal static ReadOnlyCollection<OsdbCollection> QueryCollections(this IMemoryCache cache, string query)
        => GetAllCollections(cache).Where(x => x.Name.Contains(query)).ToList().AsReadOnly();

    internal static ReadOnlyCollection<OsuBeatmap> QueryBeatmaps(this IMemoryCache cache, string query)
        => GetAllBeatmaps(cache).Where(x => x.Title.Contains(query)).ToList().AsReadOnly();

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

    internal static bool IsSame<TEnumerable, TItem, TComparer>(
        this IMemoryCache cache,
        string key,
        TEnumerable comparableEnumerable, 
        TComparer equalityComparer)
        where TEnumerable : IReadOnlyList<TItem>
        where TComparer : IEqualityComparer<TItem>
        => cache.TryGetValue(key, out TEnumerable? cachedEnumerable) && cachedEnumerable is not null && cachedEnumerable.SequenceEqual(comparableEnumerable, equalityComparer);

    internal static bool IsSame<T>(this IMemoryCache cache, string key, T comparableValue)
        => cache.TryGetValue(key, out T? cachedValue) && cachedValue is not null && cachedValue.Equals(comparableValue);
}

internal sealed class MemoryCacheKeys
{
    internal static string DatabaseFilePath => "database-filepath";

    internal static string Beatmaps => "all-beatmaps";

    internal static string Collections => "all-collections";
}
