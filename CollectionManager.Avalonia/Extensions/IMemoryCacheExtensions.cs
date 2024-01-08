using CollectionManager.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CollectionManager.Avalonia.Extensions;

internal static class IMemoryCacheExtensions
{
    internal static ReadOnlyCollection<OsdbCollection> GetAllCollections(this IMemoryCache cache)
        => cache.Get<ReadOnlyCollection<OsdbCollection>>(IMemoryCacheKeys.Collections)!;

    internal static ReadOnlyCollection<OsuBeatmap> GetAllBeatmaps(this IMemoryCache cache)
        => cache.Get<ReadOnlyCollection<OsuBeatmap>>(IMemoryCacheKeys.Beatmaps)!;

    internal static ReadOnlyCollection<OsdbCollection> QueryCollections(this IMemoryCache cache, string query)
        => GetAllCollections(cache).Where(x => x.Name.Contains(query)).ToList().AsReadOnly();

    internal static ReadOnlyCollection<OsuBeatmap> QueryBeatmaps(this IMemoryCache cache, string query)
        => GetAllBeatmaps(cache).Where(x => x.Title.Contains(query)).ToList().AsReadOnly();

    internal static void AddCollections(this IMemoryCache cache, ReadOnlyCollection<OsdbCollection> newCollections)
        => cache.Set(IMemoryCacheKeys.Collections, newCollections);

    internal static void AddBeatmaps(this IMemoryCache cache, ReadOnlyCollection<OsuBeatmap> newBeatmaps)
        => cache.Set(IMemoryCacheKeys.Beatmaps, newBeatmaps);

    internal static bool IsSame<T, TItem>(this IMemoryCache cache, string key, T comparableEnumerable) where T : IEnumerable<TItem>
        => cache.TryGetValue(key, out T? cachedEnumerable) && cachedEnumerable is not null && cachedEnumerable.SequenceEqual(comparableEnumerable);

    internal static bool IsSame<T>(this IMemoryCache cache, string key, T comparableValue)
        => cache.TryGetValue(key, out T? cachedValue) && cachedValue is not null && cachedValue.Equals(comparableValue);
}

internal sealed class IMemoryCacheKeys
{
    internal static string DatabaseFilePath => "database-filepath";

    internal static string Beatmaps => "all-beatmaps";

    internal static string Collections => "all-collections";
}
