## Generic Equality Comparer

In C#, comparing objects for equality (that is, value equality) can become a common task with much the same repetitive code. It would therefore be nice
for classes which unlike structs and records lacks built-in equality comparison.

This project contains a generic equality comparer implementation that can be used to compare objects of any type for equality. The comparer is designed to be efficient and can be used in various scenarios where object comparison is required.

### Features

- **Type Safety**: The comparer is implemented using generics, ensuring type safety at compile time.
- **Performance**: The comparer is optimized for performance and can handle large collections of objects efficiently.
- Instead of using reflection, the comparer uses expressions that are compiled into delegates, which are cached in addition,
- for fast member access and value comparison.

### Usage

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

### Conclusion

The generic equality comparer is a powerful tool for comparing objects in .NET applications. Its flexibility and performance make it a valuable addition to any developer's toolkit.