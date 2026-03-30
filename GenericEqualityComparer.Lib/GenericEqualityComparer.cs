using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace GenericEqualityComparer.Lib
{
    
    public class GenericEqualityComparer<T> : IEqualityComparer<T> where T : class
    {

        private static List<Func<T, object>> _getters = 
            new List<Func<T, object>>();

        public GenericEqualityComparer()
        {
            CreatePublicPropertyGetters();
        }

        private void CreatePublicPropertyGetters()
        {
            if (_getters.Any())
            {
                return; //use cached getters in static list for given type parameter T
            }
            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(m => m.GetMethod != null).ToArray() ;
            foreach (var prop in props) {
                ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
                MemberExpression propertyExpression = Expression.Property(parameter, prop.Name);
                Expression boxedPropertyExpression = Expression.Convert(propertyExpression, typeof(object));
                Expression<Func<T, object>> propertyGetter = Expression.Lambda<Func<T, object>>(boxedPropertyExpression, parameter);
                _getters.Add(propertyGetter.Compile());
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

            foreach (var prop in _getters)
            {
                var xv = prop(x);
                var yv = prop(y);
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

            var propertyValues = _getters.Select(p => p(obj)).ToList();

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


            return hash;              
        }

    }

}
