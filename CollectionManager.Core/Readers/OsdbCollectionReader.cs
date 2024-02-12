using CollectionManager.Core.Infrastructure;
using CollectionManager.Core.Models;
using CollectionManager.Core.Objects;
using System.Collections.Frozen;
using System.IO.Compression;
using System.Text;

namespace CollectionManager.Core.Readers;

public sealed class OsdbCollectionReader
{
    private readonly FrozenDictionary<string, int> _versions = new Dictionary<string, int>()
    {
        {"o!dm", 1},
        {"o!dm2", 2},
        {"o!dm3", 3},
        {"o!dm4", 4},
        {"o!dm5", 5},
        {"o!dm6", 6},
        {"o!dm7", 7},
        {"o!dm8", 8},
        {"o!dm7min", 1007},
        {"o!dm8min", 1008},

    }.ToFrozenDictionary();

    private int _versionKey;

    public Result<OsdbDatabase> ReadFile(string filepath)
    {
        using FileStream collectionStream = File.OpenRead(filepath);
        using MemoryStream dataStream = new();
        using BinaryReader versionReader = new(collectionStream, Encoding.UTF8, leaveOpen: true);

        string version = versionReader.ReadString();
        _versionKey = _versions.GetValueOrDefault(version);

        if (_versionKey == default)
        {
            return new InvalidDataException($"Unrecognized osdb file version (got: {version})");
        }

        if (_versionKey is 7 or 8)
        {
            using GZipStream decompressionStream = new(collectionStream, CompressionMode.Decompress);
            decompressionStream.CopyTo(dataStream);
        }
        else
        {
            versionReader.BaseStream.CopyTo(dataStream);
        }

        dataStream.Position = 0;
        using BinaryReader collectionReader = new(dataStream);

        collectionReader.ReadString(); // Version.
        DateTime date = DateTime.FromOADate(collectionReader.ReadDouble());
        string editor = collectionReader.ReadString();
        int collectionCount = collectionReader.ReadInt32();

        List<OsdbCollection> collections = new(capacity: collectionCount);

        for (int i = 0; i < collectionCount; i++)
        {
            string name = collectionReader.ReadString();
            int osuStatsId = GetValueOrDefault(collectionReader.ReadInt32, version => version is 7 or 8);
            int beatmapCount = collectionReader.ReadInt32();

            List<OsdbBeatmap> beatmaps = new(capacity: beatmapCount);

            for (int j = 0; j < beatmapCount; j++)
            {
                beatmaps.Add(new OsdbBeatmap
                (
                    Id: collectionReader.ReadInt32(),
                    BeatmapsetId: GetValueOrDefault(collectionReader.ReadInt32, version => version >= 2),
                    Artist: GetValueOrDefault(collectionReader.ReadString, version => version is not 1007 and not 1008),
                    Title: GetValueOrDefault(collectionReader.ReadString, version => version is not 1007 and not 1008),
                    DifficultyName: GetValueOrDefault(collectionReader.ReadString, version => version is not 1007 and not 1008),
                    Hash: collectionReader.ReadString(),
                    Comments: GetValueOrDefault(collectionReader.ReadString, version => version >= 4),
                    Ruleset: (Ruleset)GetValueOrDefault(collectionReader.ReadByte, version => version is >= 8 or >= 5 and not 1007 and not 1008),
                    DifficultyRating: GetValueOrDefault(collectionReader.ReadDouble, version => version is >= 8 or >= 6 and not 1007 and not 1008)
                ));
            }

            int missingBeatmapCount = 0;
            HashSet<string> missingBeatmaps = [];

            if (_versionKey >= 3)
            {
                missingBeatmapCount = collectionReader.ReadInt32();
                missingBeatmaps = new(capacity: missingBeatmapCount);

                for (int k = 0; k < missingBeatmaps.Count; k++)
                {
                    missingBeatmaps.Add(collectionReader.ReadString());
                }
            }

            collections.Add(new OsdbCollection
            (
                name,
                osuStatsId,
                beatmaps.AsReadOnly(),
                missingBeatmaps
            ));
        }

        if (collectionReader.ReadString() is not "By Piotrekol")
        {
            return new InvalidDataException("File footer is invalid, this collection might be corrupted.");
        }

        return new OsdbDatabase(version, date, editor, collections.AsReadOnly());
    }

    private T? GetValueOrDefault<T>(Func<T> input, Predicate<int> version)
        => version(_versionKey) ? input() : default;
}
