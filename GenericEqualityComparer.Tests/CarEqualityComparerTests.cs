using GenericEqualityComparer.Lib;
using GenericEqualityComparer.Models;

namespace GenericEqualityComparer.Tests;

[TestFixture]
public class CarEqualityComparerTests
{
    // -------------------------------------------------------
    // Public properties only (default comparer)
    // -------------------------------------------------------

    [TestCase("Toyota", "Camry",   2020, "Toyota", "Camry",   2020, ExpectedResult = true,  TestName = "Same make/model/year returns true")]
    [TestCase("Toyota", "Camry",   2020, "Toyota", "Corolla", 2020, ExpectedResult = false, TestName = "Different model returns false")]
    [TestCase("Toyota", "Camry",   2020, "Ford",   "Camry",   2020, ExpectedResult = false, TestName = "Different make returns false")]
    [TestCase("Toyota", "Camry",   2020, "Toyota", "Camry",   2021, ExpectedResult = false, TestName = "Different year returns false")]
    public bool Equals_PublicProperties_ReturnsExpected(
        string make1, string model1, int year1,
        string make2, string model2, int year2)
    {
        var comparer = new GenericEqualityComparer<Car>();
        return comparer.Equals(
            new Car { Make = make1, Model = model1, Year = year1 },
            new Car { Make = make2, Model = model2, Year = year2 });
    }

    // -------------------------------------------------------
    // Private field (_secretAssemblyNumberInField)
    // -------------------------------------------------------

    [Test]
    public void Equals_DefaultComparer_IgnoresPrivateField()
    {
        var car1 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
        var car2 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
        car1.SetSecretAssemblyNumber("ASM-001");
        car2.SetSecretAssemblyNumber("ASM-999");

        var comparer = new GenericEqualityComparer<Car>();

        Assert.That(comparer.Equals(car1, car2), Is.True);
    }

    [Test]
    public void Equals_WithIncludePrivateFields_DetectsPrivateFieldDifference()
    {
        var car1 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
        var car2 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
        car1.SetSecretAssemblyNumber("ASM-001");
        car2.SetSecretAssemblyNumber("ASM-999");

        var comparer = new GenericEqualityComparer<Car>(includePrivateFields: true);

        Assert.That(comparer.Equals(car1, car2), Is.False);
    }

    [Test]
    public void Equals_WithIncludePrivateFields_ReturnsTrueWhenFieldsMatch()
    {
        var car1 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
        var car2 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
        car1.SetSecretAssemblyNumber("ASM-001");
        car2.SetSecretAssemblyNumber("ASM-001");

        var comparer = new GenericEqualityComparer<Car>(includePrivateFields: true);

        Assert.That(comparer.Equals(car1, car2), Is.True);
    }
}
