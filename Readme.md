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
### EqualityWrapper<T> and the == / != operators

C# doesn't allow overloading == and != on a generic type parameter T in an external comparer class. As a workaround, GenericEqualityComparer<T> exposes a For(value) method that returns an EqualityWrapper<T>. The wrapper carries both the value and the comparer, so its == and != operators delegate to the comparer instead of defaulting to reference equality.
#### Basic operator usage

````csharp
var comparer = new GenericEqualityComparer<Car>();

var car1 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };
var car2 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };
var car3 = new Car { Make = "Toyota", Model = "Corolla", Year = 2020 };

bool same      = comparer.For(car1) == comparer.For(car2);  // True
bool different = comparer.For(car1) != comparer.For(car3);  // True
```

#### With private member detection
  
````csharp
var deepComparer = new GenericEqualityComparer<Car>(includePrivateFields: true);

var ford1 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
var ford2 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
ford1.SetSecretAssemblyNumber("ASM-001");
ford2.SetSecretAssemblyNumber("ASM-999");

if (deepComparer.For(ford1) != deepComparer.For(ford2))
{
    Console.WriteLine("Cars differ (private field detected)");
}
```

### Consistent hashing

EqualityWrapper<T> also overrides GetHashCode() so it stays consistent with ==. This means wrapped values can be used safely as dictionary keys or in hash sets.

var comparer = new GenericEqualityComparer<Car>();
var car1     = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };
var car2     = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };

int hash1 = comparer.For(car1).GetHashCode();
int hash2 = comparer.For(car2).GetHashCode();

Console.WriteLine(hash1 == hash2);  // True — equal objects, equal hashes

#### When not to use it
Performance: The comparer uses reflection to discover members at construction time (compiled to delegates for speed), but it is still a little slower than a hand-written Equals. Avoid it in tight loops or hot paths.

    Records — C# records already have value equality built in. Use == directly.
    Structs — Same as records; value equality is the default.
    Classes you own — Prefer overriding Equals / GetHashCode or implementing IEquatable<T> for production code (due to performance). Use this comparer for tests, prototyping, or third-party types you can't modify. Or if you just would like a simple way of providing value based equality checks, but in that case you should
    really
    consider a specific implementation.

In case you work with generated code or for got a large number of POCO classes (Data transfer objects) and want to avoid using inheritance or adding value equality of your existing code, this code allows you adding value based equality, this code shown here should have you covered with a generic util class. 


### Conclusion

The generic equality comparer is a powerful tool for comparing objects in .NET applications. Its flexibility and performance make it a valuable addition to any developer's toolkit.