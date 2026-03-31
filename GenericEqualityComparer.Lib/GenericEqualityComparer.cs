using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace GenericEqualityComparer.Lib
{
    
    public class GenericEqualityComparer<T> : IEqualityComparer<T> where T : class
    {

        private List<Func<T, object>> _propertyGetters = new List<Func<T, object>>();

        private List<Func<T, object>> _fieldGetters = new List<Func<T, object>>();


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
            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(m => m.GetMethod != null).ToList();
            if (includePrivateProperties)
            {
                var privateProps = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic).Where(m => m.GetMethod != null).ToList();
                props.AddRange(privateProps);
            }

            foreach (var prop in props) {
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
                ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
                MemberExpression fieldExpression = Expression.Field(parameter, field.Name);
                Expression boxedFieldExpression = Expression.Convert(fieldExpression, typeof(object));
                Expression<Func<T, object>> fieldGetter = Expression.Lambda<Func<T, object>>(boxedFieldExpression, parameter);
                _fieldGetters.Add(fieldGetter.Compile());
            }

        }



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

        public int GetHashCode([DisallowNull] T obj)
        {
            int hash = 0;

            var propertyValues = _propertyGetters.Select(p => p(obj)).ToList();

            for (int i = 0; i < propertyValues.Count; i += 8)
            {
                hash = HashCode.Combine(hash,
                    propertyValues.ElementAtOrDefault(i),
                    propertyValues.ElementAtOrDefault(i+1),
                    propertyValues.ElementAtOrDefault(i+2),
                    propertyValues.ElementAtOrDefault(i+3),
                    propertyValues.ElementAtOrDefault(i+4),
                    propertyValues.ElementAtOrDefault(i +5),
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
