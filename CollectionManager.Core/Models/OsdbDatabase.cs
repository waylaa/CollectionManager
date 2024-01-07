using CollectionManager.Core.Objects;
using System.Collections.ObjectModel;

namespace CollectionManager.Core.Models;

public sealed record OsdbDatabase
(
    string Version,
    DateTime Date,
    string Editor,
    ReadOnlyCollection<OsdbCollection> Collections
);

public sealed record OsdbCollection
(
    string Name,
    int OsuStatsId,
    ReadOnlyCollection<OsdbBeatmap> Beatmaps,
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
