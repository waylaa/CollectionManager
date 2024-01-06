using CollectionManager.Core.Objects;
using System.Collections.ObjectModel;

namespace CollectionManager.Core.Models;

public sealed record OsdbDatabase
(
    string Version,
    DateTime Date,
    string Editor,
    int CollectionCount,
    ReadOnlyCollection<OsdbCollection> Collections
);

public sealed record OsdbCollection
(
    string Name,
    int OsuStatsId,
    int BeatmapCount,
    ReadOnlyCollection<OsdbBeatmap> Beatmaps,
    int MissingBeatmapCount,
    IReadOnlySet<string> MissingBeatmaps
);

public sealed record OsdbBeatmap
(
    int Id,
    int BeatmapsetId,
    string? Artist,
    string? Title,
    string? DifficultyName,
    string Hash,
    string? Comments,
    Ruleset? Ruleset,
    double? DifficultyRating
);
