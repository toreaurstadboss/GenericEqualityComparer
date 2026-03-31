using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace GenericEqualityComparer.Lib
{

    /// <summary>
    /// A reflection-based <see cref="IEqualityComparer{T}"/> that compares instances of
    /// <typeparamref name="T"/> by their members rather than by reference.
    /// This is intended for use with classes that do not implement their own value-based equality semantics, and is not recommended for performance-sensitive scenarios.
    /// Types such as structs and records already have built-in value equality semantics and should not require this comparer.
    /// </summary>
    /// <typeparam name="T">The type to compare. Must be a reference type.</typeparam>
    /// <remarks>
    /// By default only public instance properties are compared. Pass the constructor flags to
    /// also include private properties and/or fields (public or private).
    /// </remarks>
    public class GenericEqualityComparer<T> : IEqualityComparer<T> where T : class
    {

        private List<Func<T, object>> _propertyGetters = new List<Func<T, object>>(); // Cache of compiled delegates for accessing the configured properties of T, used to avoid the performance overhead of reflection during comparisons.

        private List<Func<T, object>> _fieldGetters = new List<Func<T, object>>(); // Cache of compiled delegates for accessing the configured fields of T, used to avoid the performance overhead of reflection during comparisons.

        /// <summary>
        /// Initialises the comparer and builds the member accessor cache.
        /// </summary>
        /// <param name="includeFields">When <see langword="true"/>, public instance fields are included in the comparison.</param>
        /// <param name="includePrivateProperties">When <see langword="true"/>, private instance properties are included in the comparison.</param>
        /// <param name="includePrivateFields">When <see langword="true"/>, private instance fields are included in the comparison. Also enables public field comparison.</param>
        public GenericEqualityComparer(bool includeFields = false, bool includePrivateProperties = false, bool includePrivateFields = false)
        {
            CreatePropertyGetters(includePrivateProperties);
            if (includeFields || includePrivateFields)
            {
                CreateFieldGetters(includePrivateFields);
            }
        }

        private void CreatePropertyGetters(bool includePrivateProperties)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            if (includePrivateProperties)
            {
                bindingFlags |= BindingFlags.NonPublic;
            }

            var props = typeof(T).GetProperties(bindingFlags).Where(m => m.GetMethod != null).ToList();

            foreach (var prop in props)
            {

                //Builds the Expression<Func<T, object>> for the property getter and compiles it into a Func<T, object> delegate, which is cached for later use.
                ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
                MemberExpression propertyExpression = Expression.Property(parameter, prop.Name);
                Expression boxedPropertyExpression = Expression.Convert(propertyExpression, typeof(object));
                Expression<Func<T, object>> propertyGetter = Expression.Lambda<Func<T, object>>(boxedPropertyExpression, parameter);
                _propertyGetters.Add(propertyGetter.Compile());
            }
        }

        private void CreateFieldGetters(bool includePrivateFields)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            if (includePrivateFields)
            {
                bindingFlags |= BindingFlags.NonPublic;
            }

            var fields = typeof(T).GetFields(bindingFlags).ToList();

            foreach (var field in fields)
            {
                // Builds the Expression<Func<T, object>> for the field getter and compiles it into a Func<T, object> delegate, which is cached for later use.
                ParameterExpression parameter = Expression.Parameter(typeof(T), "f");
                MemberExpression fieldExpression = Expression.Field(parameter, field.Name);
                Expression boxedPropertyExpression = Expression.Convert(fieldExpression, typeof(object));
                Expression<Func<T, object>> fieldGetter = Expression.Lambda<Func<T, object>>(boxedPropertyExpression, parameter);
                _fieldGetters.Add(fieldGetter.Compile());
            }

        }

        /// <summary>
        /// Determines whether <paramref name="x"/> and <paramref name="y"/> are equal by comparing
        /// each configured member in turn.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// <see langword="true"/> when all configured members are equal;
        /// <see langword="false"/> when any member differs, or either argument is <see langword="null"/>.
        /// </returns>
        public bool Equals(T? x, T? y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (x.GetType() != y.GetType())
            {
                return false;
            }

            foreach (var propAccessor in _propertyGetters)
            {
                var xv = propAccessor(x);
                var yv = propAccessor(y);
                if (!xv.Equals(yv))
                {
                    return false;
                }
            }

            foreach (var fieldAccessor in _fieldGetters)
            {
                var xv = fieldAccessor(x);
                var yv = fieldAccessor(y);
                if (!xv.Equals(yv))
                {
                    return false;
                }

            }


            return true;
        }

        /// <summary>
        /// Returns an <see cref="EqualityWrapper{T}"/> for <paramref name="value"/> so that
        /// <c>==</c> and <c>!=</c> use this comparer's configured equality semantics.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <returns>An <see cref="EqualityWrapper{T}"/> bound to this comparer instance.</returns>
        public EqualityWrapper<T> For(T value) => new EqualityWrapper<T>(value, this);

        /// <summary>
        /// Returns a hash code for <paramref name="obj"/> derived from the same configured members
        /// used by <see cref="Equals(T, T)"/>.
        /// </summary>
        /// <param name="obj">The object to hash.</param>
        /// <returns>A hash code consistent with the configured equality semantics.</returns>
        public int GetHashCode([DisallowNull] T obj)
        {
            int hash = 0;

            var propertyValues = _propertyGetters.Select(p => p(obj)).ToList();

            for (int i = 0; i < propertyValues.Count; i += 8)
            {
                hash = HashCode.Combine(hash,
                    propertyValues.ElementAtOrDefault(i),
                    propertyValues.ElementAtOrDefault(i + 1),
                    propertyValues.ElementAtOrDefault(i + 2),
                    propertyValues.ElementAtOrDefault(i + 3),
                    propertyValues.ElementAtOrDefault(i + 4),
                    propertyValues.ElementAtOrDefault(i + 5),
                    propertyValues.ElementAtOrDefault(i + 6));
            }

            if (_fieldGetters.Any())
            {
                var fieldValues = _fieldGetters.Select(f => f(obj)).ToList();
                for (int i = 0; i < fieldValues.Count; i += 8)
                {
                    hash = HashCode.Combine(hash,
                        fieldValues.ElementAtOrDefault(i),
                        fieldValues.ElementAtOrDefault(i + 1),
                        fieldValues.ElementAtOrDefault(i + 2),
                        fieldValues.ElementAtOrDefault(i + 3),
                        fieldValues.ElementAtOrDefault(i + 4),
                        fieldValues.ElementAtOrDefault(i + 5),
                        fieldValues.ElementAtOrDefault(i + 6));
                }
            }

            return hash;
        }

    }

}
