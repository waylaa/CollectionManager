using Avalonia.Markup.Xaml;
using System;

namespace CollectionManager.Avalonia.Converters.Abstractions;

public abstract class MarkupExtensionImpl : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
