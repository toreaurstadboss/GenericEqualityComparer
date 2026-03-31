using GenericEqualityComparer.Lib;
using GenericEqualityComparer.Models;

namespace GenericEqualityComparer.Tests;

[TestFixture]
public class BicycleEqualityComparerTests
{
    // -------------------------------------------------------
    // Public properties (default comparer)
    // -------------------------------------------------------

    [TestCase("Trek", "FX3", "Trek", "FX3",  ExpectedResult = true,  TestName = "Same brand and model returns true")]
    [TestCase("Trek", "FX3", "Trek", "FX2",  ExpectedResult = false, TestName = "Different model returns false")]
    [TestCase("Trek", "FX3", "Giant", "FX3", ExpectedResult = false, TestName = "Different brand returns false")]
    public bool Equals_PublicProperties_ReturnsExpected(
        string brand1, string model1,
        string brand2, string model2)
    {
        var comparer = new GenericEqualityComparer<Bicycle>();
        return comparer.Equals(
            new Bicycle { Brand = brand1, Model = model1 },
            new Bicycle { Brand = brand2, Model = model2 });
    }

    // -------------------------------------------------------
    // Private property (FrameSerialNumber)
    // -------------------------------------------------------

    [Test]
    public void Equals_DefaultComparer_IgnoresPrivateProperty()
    {
        var bike1 = new Bicycle { Brand = "Trek", Model = "FX3" };
        var bike2 = new Bicycle { Brand = "Trek", Model = "FX3" };
        bike1.SetFrameSerialNumber("SN-001");
        bike2.SetFrameSerialNumber("SN-999");

        var comparer = new GenericEqualityComparer<Bicycle>();

        Assert.That(comparer.Equals(bike1, bike2), Is.True);
    }

    [Test]
    public void Equals_WithIncludePrivateProperties_DetectsPrivatePropertyDifference()
    {
        var bike1 = new Bicycle { Brand = "Trek", Model = "FX3" };
        var bike2 = new Bicycle { Brand = "Trek", Model = "FX3" };
        bike1.SetFrameSerialNumber("SN-001");
        bike2.SetFrameSerialNumber("SN-999");

        var comparer = new GenericEqualityComparer<Bicycle>(includePrivateProperties: true);

        Assert.That(comparer.Equals(bike1, bike2), Is.False);
    }

    [Test]
    public void Equals_WithIncludePrivateProperties_ReturnsTrueWhenPropertyMatches()
    {
        var bike1 = new Bicycle { Brand = "Trek", Model = "FX3" };
        var bike2 = new Bicycle { Brand = "Trek", Model = "FX3" };
        bike1.SetFrameSerialNumber("SN-001");
        bike2.SetFrameSerialNumber("SN-001");

        var comparer = new GenericEqualityComparer<Bicycle>(includePrivateProperties: true);

        Assert.That(comparer.Equals(bike1, bike2), Is.True);
    }

    // -------------------------------------------------------
    // Private field (_factoryCode)
    // -------------------------------------------------------

    [Test]
    public void Equals_DefaultComparer_IgnoresPrivateField()
    {
        var bike1 = new Bicycle { Brand = "Giant", Model = "Escape" };
        var bike2 = new Bicycle { Brand = "Giant", Model = "Escape" };
        bike1.SetFactoryCode("FACTORY-A");
        bike2.SetFactoryCode("FACTORY-B");

        var comparer = new GenericEqualityComparer<Bicycle>();

        Assert.That(comparer.Equals(bike1, bike2), Is.True);
    }

    [Test]
    public void Equals_WithIncludePrivateFields_DetectsPrivateFieldDifference()
    {
        var bike1 = new Bicycle { Brand = "Giant", Model = "Escape" };
        var bike2 = new Bicycle { Brand = "Giant", Model = "Escape" };
        bike1.SetFactoryCode("FACTORY-A");
        bike2.SetFactoryCode("FACTORY-B");

        var comparer = new GenericEqualityComparer<Bicycle>(includePrivateFields: true);

        Assert.That(comparer.Equals(bike1, bike2), Is.False);
    }

    [Test]
    public void Equals_WithIncludePrivateFields_ReturnsTrueWhenFieldsMatch()
    {
        var bike1 = new Bicycle { Brand = "Giant", Model = "Escape" };
        var bike2 = new Bicycle { Brand = "Giant", Model = "Escape" };
        bike1.SetFactoryCode("FACTORY-A");
        bike2.SetFactoryCode("FACTORY-A");

        var comparer = new GenericEqualityComparer<Bicycle>(includePrivateFields: true);

        Assert.That(comparer.Equals(bike1, bike2), Is.True);
    }
}
