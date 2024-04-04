using CollectionManager.Core.Infrastructure;
using CollectionManager.Core.Models;
using CollectionManager.Core.Objects;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace CollectionManager.Core.Readers;

public sealed class OsuDatabaseReader
{
    [NotNull]
    private BinaryReader? _databaseReader = null;

    private int _databaseVersion;

    public Result<OsuDatabase> ReadFile(string filepath)
    {
        try
        {
            using FileStream databaseStream = File.OpenRead(filepath);
            _databaseReader = new(databaseStream);

            _databaseVersion = _databaseReader.ReadInt32();
            
            _databaseReader.BaseStream.Seek(sizeof(int) + sizeof(bool) + sizeof(long), SeekOrigin.Current); // FolderCount, IsAccountUnlocked, AccountUnlockDate.
            ReadStringConditionally(); // Username.

            int beatmapCount = _databaseReader.ReadInt32();
            List<OsuBeatmap> beatmaps = new(capacity: beatmapCount);

            for (int i = 0; i < beatmapCount; i++)
            {
                if (_databaseVersion < 20191106)
                {
                    _databaseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Current); // Size in bytes of the beatmap entry.
                }

                string artist = ReadStringConditionally();
                ReadStringConditionally(); // Artist in unicode.

                string title = ReadStringConditionally();
                ReadStringConditionally(); // Title in unicode.
                ReadStringConditionally(); // Creator.

                string difficultyName = ReadStringConditionally();
                string audioFileName = ReadStringConditionally();
                string hash = ReadStringConditionally();

                ReadStringConditionally(); // Corresponding .osu file.

                Status rankStatus = (Status)_databaseReader.ReadByte();

                _databaseReader.BaseStream.Seek((sizeof(short) * 3) + sizeof(long), SeekOrigin.Current); // Circle, Slider, Spinner count, last modification time.

                object approachRate = GetValueOrAlternative(_databaseReader.ReadSingle, _databaseReader.ReadByte, version => version > 20140609);
                object circleSize = GetValueOrAlternative(_databaseReader.ReadSingle, _databaseReader.ReadByte, version => version > 20140609);
                object hpDrain = GetValueOrAlternative(_databaseReader.ReadSingle, _databaseReader.ReadByte, version => version > 20140609);
                object overallDifficulty = GetValueOrAlternative(_databaseReader.ReadSingle, _databaseReader.ReadByte, version => version > 20140609);

                _databaseReader.BaseStream.Seek(sizeof(double), SeekOrigin.Current); // Slider velocity.

                KeyValuePair<Ruleset, FrozenDictionary<Mods, double>>? standardModCombinatedStarRatings = GetValueOrDefault(() => ReadModCombinatedStarRatings(Ruleset.Osu), version => version >= 20140609);
                KeyValuePair<Ruleset, FrozenDictionary<Mods, double>>? taikoModCombinatedStarRatings = GetValueOrDefault(() => ReadModCombinatedStarRatings(Ruleset.Taiko), version => version >= 20140609);
                KeyValuePair<Ruleset, FrozenDictionary<Mods, double>>? ctbModCombinatedStarRatings = GetValueOrDefault(() => ReadModCombinatedStarRatings(Ruleset.Ctb), version => version >= 20140609);
                KeyValuePair<Ruleset, FrozenDictionary<Mods, double>>? maniaModCombinatedStarRatings = GetValueOrDefault(() => ReadModCombinatedStarRatings(Ruleset.Mania), version => version >= 20140609);

                _databaseReader.BaseStream.Seek(sizeof(int) * 3, SeekOrigin.Current); // Drain time in seconds, total time, audio preview in milliseconds.

                (double minBpm, double maxBpm) bpm = ReadBpm();
                int id = _databaseReader.ReadInt32();
                int beatmapsetId = _databaseReader.ReadInt32();

                _databaseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Current); // ThreadId.

                Grade standardGrade = (Grade)_databaseReader.ReadByte();
                Grade taikoGrade = (Grade)_databaseReader.ReadByte();
                Grade ctbGrade = (Grade)_databaseReader.ReadByte();
                Grade maniaGrade = (Grade)_databaseReader.ReadByte();

                _databaseReader.BaseStream.Seek(sizeof(short) + sizeof(float) + sizeof(byte), SeekOrigin.Current); // Local offset, stack leniency, ruleset.
                ReadStringConditionally(); // Song source
                ReadStringConditionally(); // Song tags.
                _databaseReader.BaseStream.Seek(sizeof(short), SeekOrigin.Current); // Online offset.
                ReadStringConditionally(); // Title font.
                _databaseReader.BaseStream.Seek(sizeof(bool), SeekOrigin.Current); // Is beatmap unplayed.

                DateTime lastPlayed = ReadDateTime();

                _databaseReader.BaseStream.Seek(sizeof(bool), SeekOrigin.Current); // Is osz2.

                string folderName = ReadStringConditionally();
                DateTime lastUpdated = ReadDateTime();

                // Is beatmap sound/skin ignored, is storyboard disabled, is video disabled, is visually overriden.
                _databaseReader.BaseStream.Seek(sizeof(bool) * 5, SeekOrigin.Current);

                if (_databaseVersion < 20140609)
                {
                    _databaseReader.BaseStream.Seek(sizeof(short), SeekOrigin.Current); // Skip unknown variable.
                }

                _databaseReader.BaseStream.Seek(sizeof(int) + sizeof(byte), SeekOrigin.Current); // Skip another last modification time, mania scroll speed.

                beatmaps.Add(new OsuBeatmap
                (
                    artist,
                    title,
                    difficultyName,
                    audioFileName,
                    hash,
                    rankStatus,
                    approachRate,
                    circleSize,
                    hpDrain,
                    overallDifficulty,
                    standardModCombinatedStarRatings,
                    taikoModCombinatedStarRatings,
                    ctbModCombinatedStarRatings,
                    maniaModCombinatedStarRatings,
                    bpm,
                    id,
                    id,
                    standardGrade,
                    taikoGrade,
                    ctbGrade,
                    maniaGrade,
                    lastPlayed,
                    folderName,
                    lastUpdated
                ));
            }

            _databaseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Current);

            return new OsuDatabase(_databaseVersion, beatmaps.AsReadOnly());
        }
        catch (Exception ex)
        {
            return ex;
        }
        finally
        {
            _databaseReader.Dispose();
        }
    }

    private KeyValuePair<Ruleset, FrozenDictionary<Mods, double>>? ReadModCombinatedStarRatings(Ruleset ruleset)
    {
        int modCombinatedStarRatingsCount = _databaseReader.ReadInt32();

        if (modCombinatedStarRatingsCount <= 0)
        {
            return null;
        }

        Dictionary<Mods, double> modCombinatedStarRatings = new(capacity: modCombinatedStarRatingsCount);

        for (int i = 0; i < modCombinatedStarRatingsCount; i++)
        {
            if (ReadConditionally() is not object modsEnum)
            {
                continue;
            }

            Mods mods = (Mods)modsEnum;
            double starRating = Math.Round(Convert.ToDouble(ReadConditionally()), digits: 2);

            if (starRating == 0)
            {
                continue;
            }

            if (!modCombinatedStarRatings.TryGetValue(mods, out double cachedStarRating))
            {
                modCombinatedStarRatings.Add(mods, starRating);
            }
            else
            {
                if (cachedStarRating < starRating)
                {
                    modCombinatedStarRatings[mods] = starRating;
                }
            }
        }

        return KeyValuePair.Create(ruleset, modCombinatedStarRatings.ToFrozenDictionary());
    }

    private (double minBpm, double maxBpm) ReadBpm()
    {
        _databaseReader.BaseStream.Seek(-(sizeof(int) * 2), SeekOrigin.Current);
        int totalTimeInMs = _databaseReader.ReadInt32();
        _databaseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Current);

        int timingPointsCount = _databaseReader.ReadInt32();
        List<TimingPoint> timingPoints = new(capacity: timingPointsCount);

        for (int i = 0; i < timingPointsCount; i++)
        {
            timingPoints.Add(new TimingPoint
            (
                BpmDuration: _databaseReader.ReadDouble(),
                Offset: _databaseReader.ReadDouble(),
                IsInheritingBpm: _databaseReader.ReadBoolean()
            ));
        }

        double minBpm = double.MinValue;
        double maxBpm = double.MaxValue;
        double currentBpmLength = 0;
        double lastTime = totalTimeInMs;

        Dictionary<double, int> bpmTimes = [];
        
        for (int i = timingPoints.Count - 1; i >= 0; i--)
        {
            TimingPoint timingPoint = timingPoints[i];

            if (timingPoint.IsInheritingBpm)
            {
                currentBpmLength = timingPoint.BpmDuration;
            }

            if (currentBpmLength == 0 || timingPoint.Offset > lastTime || (!timingPoint.IsInheritingBpm && i > 0))
            {
                continue;
            }

            if (currentBpmLength > minBpm)
            {
                minBpm = currentBpmLength;
            }

            if (currentBpmLength < maxBpm)
            {
                maxBpm = currentBpmLength;
            }

            if (!bpmTimes.ContainsKey(currentBpmLength))
            {
                bpmTimes[currentBpmLength] = 0;
            }

            bpmTimes[currentBpmLength] += (int)(lastTime - (i == 0 ? 0 : timingPoint.Offset));
            lastTime = timingPoint.Offset;
        }

        const int secondInMs = 60_000;
        return (Math.Round(secondInMs / minBpm), Math.Round(secondInMs / maxBpm));
    }

    private string ReadStringConditionally()
        => _databaseReader.ReadByte() == 11 ? _databaseReader.ReadString() : string.Empty;

    private ReadOnlyMemory<char> ReadCharsConditionally()
    {
        int length = _databaseReader.ReadInt32();
        return length > 0 ? _databaseReader.ReadChars(length) : ReadOnlyMemory<char>.Empty;
    }

    private ReadOnlyMemory<byte> ReadBytesConditionally()
    {
        int length = _databaseReader.ReadInt32();
        return length > 0 ? _databaseReader.ReadBytes(length) : ReadOnlyMemory<byte>.Empty;
    }

    private object? ReadConditionally() => _databaseReader.ReadByte() switch
    {
        1 => _databaseReader.ReadBoolean(),
        2 => _databaseReader.ReadByte(),
        3 => _databaseReader.ReadUInt16(),
        4 => _databaseReader.ReadUInt32(),
        5 => _databaseReader.ReadUInt64(),
        6 => _databaseReader.ReadSByte(),
        7 => _databaseReader.ReadInt16(),
        8 => _databaseReader.ReadInt32(),
        9 => _databaseReader.ReadInt64(),
        10 => _databaseReader.ReadChar(),
        11 => _databaseReader.ReadString(),
        12 => _databaseReader.ReadSingle(),
        13 => _databaseReader.ReadDouble(),
        14 => _databaseReader.ReadDecimal(),
        15 => ReadDateTime(),
        16 => ReadBytesConditionally(),
        17 => ReadCharsConditionally(),
        _ => null
    };

    private DateTime ReadDateTime()
    {
        long ticks = _databaseReader.ReadInt64();

        return ticks < 0L || ticks > DateTime.MaxValue.Ticks || ticks < DateTime.MinValue.Ticks
            ? DateTime.MinValue
            : new DateTime(ticks, DateTimeKind.Utc);
    }

    private object GetValueOrAlternative<T, TAlt>(Func<T> input, Func<TAlt> altInput, Predicate<int> version)
        where T : notnull
        where TAlt : notnull
        => version(_databaseVersion) ? input() : altInput();

    private T? GetValueOrDefault<T>(Func<T> input, Predicate<int> version)
        => version(_databaseVersion) ? input() : default;
}
