## Generic Equality Comparer and Equality wrapper

In C#, comparing objects for equality (that is, value equality) can become a common task with much the same repetitive code. It would therefore be nice
for classes which unlike structs and records lacks built-in equality comparison.

This project contains a generic equality comparer implementation that can be used to compare objects of any type for equality. The comparer is designed to be efficient and can be used in various scenarios where object comparison is required.

In addition, using the method `For` allows you to create an equality wrapper for a specific type, which can be used to compare objects of that type for equality. This can be particularly useful when you want to compare objects in collections or when you want to use the comparer in LINQ queries.

### Features

- **Type Safety**: The comparer is implemented using generics, ensuring type safety at compile time.
- **Performance**: The comparer is optimized for performance and can handle large collections of objects efficiently.
- Instead of using reflection, the comparer uses expressions that are compiled into delegates, which are cached in addition,
- for fast member access and value comparison.

### Frameworks supported
The generic equality comparer shown here could be used with netstandard 2.1 and .NET Core 2.1 or later.
In case you got .NET Framework 4.8 or earlier, you swap out using the GetHashCode method using
`HashCode.Combine` with a custom implementation of the hash code generation, which can be done using a simple algorithm that combines the hash codes of the individual properties and fields of the object.

An example of such a calculation is shown below, using selected prime numbers: 

```csharp

// Change the GetHashCode method of the GenericEqualityComparer class to the following implementation:

public int GetHashCode([DisallowNull] T obj)
{
    int hash = 17;

    var propertyValues = _propertyGetters.Select(p => p(obj)).ToList();

    for (int i = 0; i < propertyValues.Count; i += 8)
    {
        hash = Combine(hash,
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
            hash = Combine(hash,
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

private static int Combine(params object[] values)
{
    unchecked
    {
        int hash = 17;
        foreach (var v in values)
        {
            int h = v?.GetHashCode() ?? 0;
            hash = hash * 31 + h;
        }
        return hash;
    }
}


```

### Usage of the Generic EqualityCommparer

To use the generic equality comparer, simply create an instance of the `GenericEqualityComparer<T>` class, where `T` is the type of objects you want to compare. Then, use the `Equals` method to compare two objects for equality.

```csharp
var carEqualityComparer = new GenericEqualityComparer<Car>();
bool areEqual = carEqualityComparer.Equals(car1, car2);
```

### Additional configuration

It is also possible to configure the comparer to inspect private (instance) properties and fields, 
which can be useful in certain scenarios where the internal state of an object is relevant for equality comparison.


#### Including private properties
```csharp
var carEqualityComparer = new GenericEqualityComparer<Car>(includePrivateProperties: true);
bool areEqual = carEqualityComparer.Equals(car1, car2);
```

#### Including private fields
```csharp

var carEqualityComparer = new GenericEqualityComparer<Car>(includePrivateFields: true);
bool areEqual = carEqualityComparer.Equals(car1, car2);
```	

#### Including both private properties and fields
```csharp	
var carEqualityComparer = new GenericEqualityComparer<Car>(includePrivateProperties: true, includePrivateFields: true);
bool areEqual = carEqualityComparer.Equals(car1, car2);
```	

### Extended usage with the Equality wrapper
To create an equality wrapper for a specific type, use the `For` method of the `GenericEqualityComparer` class. This will return an instance of the `Equality<T>` class, which can be used to compare objects of that type for equality.

```csharp
var carEqualityComparer = new GenericEqualityComparer<Car>();
var carWrapper1 = carEqualityComparer.For(car1);
var carWrapper2 = carEqualityComparer.For(car2);
bool areEqual = carWrapper1.Equals(carWrapper2);
``` 


### Conclusion

The generic equality comparer is a powerful tool for comparing objects in .NET applications. Its flexibility and performance make it a valuable addition to any developer's toolkit.