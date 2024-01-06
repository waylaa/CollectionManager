using System.Diagnostics.CodeAnalysis;

namespace CollectionManager.Core.Infrastructure;

public readonly record struct Result<T>(T? Value, Exception? Error)
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccessful => this != default && Value is not null && Error is null;

    public bool ContainsError => this != default && Error is not null;

    public static implicit operator Result<T>(T value) => new(value, null);

    public static implicit operator Result<T>(Exception error) => new(default, error);
}
