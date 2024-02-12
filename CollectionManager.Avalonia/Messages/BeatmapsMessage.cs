using CollectionManager.Core.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;

namespace CollectionManager.Avalonia.Messages;

internal sealed class BeatmapsMessage(ReadOnlyCollection<OsuBeatmap> newBeatmaps) : ValueChangedMessage<ReadOnlyCollection<OsuBeatmap>>(newBeatmaps);
