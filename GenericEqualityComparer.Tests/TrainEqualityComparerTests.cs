using GenericEqualityComparer.Lib;
using GenericEqualityComparer.Models;

namespace GenericEqualityComparer.Tests;

[TestFixture]
public class TrainEqualityComparerTests
{
    // -------------------------------------------------------
    // Public properties including enum field (default comparer)
    // -------------------------------------------------------

    [TestCase("R7", "Type76", 6, FuelType.DieselElectric,
              "R7", "Type76", 6, FuelType.DieselElectric, ExpectedResult = true, TestName = "Identical trains return true")]
    [TestCase("R7", "Type76", 6, FuelType.DieselElectric,
              "R7", "Type76", 6, FuelType.Diesel, ExpectedResult = false, TestName = "Different fuel type returns false")]
    [TestCase("R7", "Type76", 6, FuelType.DieselElectric,
              "R8", "Type76", 6, FuelType.DieselElectric, ExpectedResult = false, TestName = "Different route returns false")]
    [TestCase("R7", "Type76", 6, FuelType.DieselElectric,
              "R7", "Type76", 4, FuelType.DieselElectric, ExpectedResult = false, TestName = "Different car count returns false")]
    [TestCase("R7", "Type76", 6, FuelType.Electric,
              "R7", "Type76", 6, FuelType.Steam, ExpectedResult = false, TestName = "Electric vs Steam returns false")]
    public bool Equals_PublicProperties_ReturnsExpected(
        string route1, string type1, int cars1, FuelType fuel1,
        string route2, string type2, int cars2, FuelType fuel2)
    {
        var comparer = new GenericEqualityComparer<Train>();
        return comparer.Equals(
            new Train { RouteName = route1, TrainTypeDesignation = type1, NumberOfCars = cars1, FuelType = fuel1 },
            new Train { RouteName = route2, TrainTypeDesignation = type2, NumberOfCars = cars2, FuelType = fuel2 });
    }

    // -------------------------------------------------------
    // Private field (_secretTrainCallsignInField)
    // -------------------------------------------------------

    [Test]
    public void Equals_DefaultComparer_IgnoresPrivateField()
    {
        var train1 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.DieselElectric };
        var train2 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.DieselElectric };
        train1.SetSecretTrainCallSign("ALPHA");
        train2.SetSecretTrainCallSign("BRAVO");

        var comparer = new GenericEqualityComparer<Train>();

        Assert.That(comparer.Equals(train1, train2), Is.True);
    }

    [Test]
    public void Equals_WithIncludePrivateFields_DetectsPrivateFieldDifference()
    {
        var train1 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.DieselElectric };
        var train2 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.DieselElectric };
        train1.SetSecretTrainCallSign("ALPHA");
        train2.SetSecretTrainCallSign("BRAVO");

        var comparer = new GenericEqualityComparer<Train>(includePrivateFields: true);

        Assert.That(comparer.Equals(train1, train2), Is.False);
    }

    [Test]
    public void Equals_WithIncludePrivateFields_ReturnsTrueWhenFieldsMatch()
    {
        var train1 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.DieselElectric };
        var train2 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.DieselElectric };
        train1.SetSecretTrainCallSign("ALPHA");
        train2.SetSecretTrainCallSign("ALPHA");

        var comparer = new GenericEqualityComparer<Train>(includePrivateFields: true);

        Assert.That(comparer.Equals(train1, train2), Is.True);
    }


}
