﻿using CollectionManager.Core.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;

namespace CollectionManager.Avalonia.Messages;

internal sealed class CollectionsMessage(ReadOnlyCollection<OsdbCollection> newCollections) : ValueChangedMessage<ReadOnlyCollection<OsdbCollection>>(newCollections);
