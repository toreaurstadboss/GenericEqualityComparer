using GenericEqualityComparer.Lib;
using GenericEqualityComparer.Models;

namespace GenericEqualityComparer.Tests;

[TestFixture]
public class EqualityWrapperOperatorTests
{
    // -------------------------------------------------------
    // == / != with public properties (Car)
    // -------------------------------------------------------

    [Test]
    public void OperatorEquals_SameCar_ReturnsTrue()
    {
        var comparer = new GenericEqualityComparer<Car>();
        var car1 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };
        var car2 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };

        Assert.That(comparer.For(car1) == comparer.For(car2), Is.True);
    }

    [Test]
    public void OperatorNotEquals_DifferentModel_ReturnsTrue()
    {
        var comparer = new GenericEqualityComparer<Car>();
        var car1 = new Car { Make = "Toyota", Model = "Camry",   Year = 2020 };
        var car2 = new Car { Make = "Toyota", Model = "Corolla", Year = 2020 };

        Assert.That(comparer.For(car1) != comparer.For(car2), Is.True);
    }

    // -------------------------------------------------------
    // == / != with private field (Car)
    // -------------------------------------------------------

    [Test]
    public void OperatorEquals_DefaultComparer_IgnoresPrivateField()
    {
        var comparer = new GenericEqualityComparer<Car>();
        var car1 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
        var car2 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
        car1.SetSecretAssemblyNumber("ASM-001");
        car2.SetSecretAssemblyNumber("ASM-999");

        // private field differs, but default comparer ignores it
        Assert.That(comparer.For(car1) == comparer.For(car2), Is.True);
    }

    [Test]
    public void OperatorNotEquals_WithIncludePrivateFields_DetectsFieldDifference()
    {
        var comparer = new GenericEqualityComparer<Car>(includePrivateFields: true);
        var car1 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
        var car2 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
        car1.SetSecretAssemblyNumber("ASM-001");
        car2.SetSecretAssemblyNumber("ASM-999");

        Assert.That(comparer.For(car1) != comparer.For(car2), Is.True);
    }

    // -------------------------------------------------------
    // == / != with private property (Bicycle)
    // -------------------------------------------------------

    [Test]
    public void OperatorNotEquals_WithIncludePrivateProperties_DetectsPropertyDifference()
    {
        var comparer = new GenericEqualityComparer<Bicycle>(includePrivateProperties: true);
        var bike1 = new Bicycle { Brand = "Trek", Model = "FX3" };
        var bike2 = new Bicycle { Brand = "Trek", Model = "FX3" };
        bike1.SetFrameSerialNumber("SN-001");
        bike2.SetFrameSerialNumber("SN-999");

        Assert.That(comparer.For(bike1) != comparer.For(bike2), Is.True);
    }

    // -------------------------------------------------------
    // Equals() / GetHashCode() contract on EqualityWrapper<T>
    // -------------------------------------------------------

    [Test]
    public void Equals_BoxedEqualityWrapper_DelegatesToComparer()
    {
        var comparer = new GenericEqualityComparer<Car>();
        var car1 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };
        var car2 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };

        object boxed1 = comparer.For(car1);
        object boxed2 = comparer.For(car2);

        Assert.That(boxed1.Equals(boxed2), Is.True);
    }

    [Test]
    public void GetHashCode_EqualCars_ReturnSameHash()
    {
        var comparer = new GenericEqualityComparer<Car>();
        var car1 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };
        var car2 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };

        Assert.That(comparer.For(car1).GetHashCode(), Is.EqualTo(comparer.For(car2).GetHashCode()));
    }
}
