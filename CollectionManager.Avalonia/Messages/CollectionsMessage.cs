using CollectionManager.Core.Models;
using System.Collections.ObjectModel;

namespace CollectionManager.Avalonia.Messages;

internal sealed record CollectionsMessage(ReadOnlyCollection<OsdbCollection> Collections);
