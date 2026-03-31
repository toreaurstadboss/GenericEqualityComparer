namespace GenericEqualityComparer.Lib;

/// <summary>
/// Pairs a value of type <typeparamref name="T"/> with a <see cref="GenericEqualityComparer{T}"/>
/// so that <c>==</c> and <c>!=</c> use the comparer's configured equality semantics instead of
/// reference equality.
/// </summary>
/// <typeparam name="T">The type of the wrapped value. Must be a reference type.</typeparam>
/// <remarks>
/// Obtain an instance via <see cref="GenericEqualityComparer{T}.For"/>:
/// <code>comparer.For(car1) == comparer.For(car2)</code>
/// </remarks>
public readonly struct EqualityWrapper<T> where T : class
{
    private readonly T _value;
    private readonly GenericEqualityComparer<T> _comparer;

    internal EqualityWrapper(T value, GenericEqualityComparer<T> comparer)
    {
        _value = value;
        _comparer = comparer;
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="left"/> and <paramref name="right"/>
    /// are considered equal by their shared comparer.
    /// </summary>
    public static bool operator ==(EqualityWrapper<T> left, EqualityWrapper<T> right)
        => left._comparer.Equals(left._value, right._value);

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="left"/> and <paramref name="right"/>
    /// are not considered equal by their shared comparer.
    /// </summary>
    public static bool operator !=(EqualityWrapper<T> left, EqualityWrapper<T> right)
        => !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is EqualityWrapper<T> other && this == other;

    /// <inheritdoc/>
    public override int GetHashCode()
        => _comparer.GetHashCode(_value);
}
