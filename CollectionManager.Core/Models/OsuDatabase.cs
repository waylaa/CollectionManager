using CollectionManager.Core.Objects;
using System.Collections.ObjectModel;

namespace CollectionManager.Core.Models;

public sealed record OsuDatabase
(
    int Version,
    ReadOnlyCollection<OsuBeatmap> Beatmaps
);

public sealed record OsuBeatmap
(
    string Artist,
    string Title,
    string DifficultyName,
    string AudioFileName,
    string Hash,
    Status RankStatus,
    object ApproachRate,
    object CircleSize,
    object HpDrain,
    object OverallDifficulty,
    KeyValuePair<Ruleset, ReadOnlyDictionary<Mods, double>>? StandardStarRatings,
    KeyValuePair<Ruleset, ReadOnlyDictionary<Mods, double>>? TaikoStarRatings,
    KeyValuePair<Ruleset, ReadOnlyDictionary<Mods, double>>? CtbStarRatings,
    KeyValuePair<Ruleset, ReadOnlyDictionary<Mods, double>>? ManiaStarRatings,
    (double minBpm, double maxBpm) Bpm,
    int DifficultyId,
    int Id,
    Grade StandardGrade,
    Grade TaikoGrade,
    Grade CtbGrade,
    Grade ManiaGrade,
    DateTime LastPlayed,
    string FolderName,
    DateTime LastUpdated
);

internal sealed record TimingPoint(double BpmDuration, double Offset, bool IsInheritingBpm);
