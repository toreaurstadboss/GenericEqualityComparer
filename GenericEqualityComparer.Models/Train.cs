namespace GenericEqualityComparer.Models;

public class Train
{
    public string? RouteName { get; set; }
    public string? TrainTypeDesignation { get; set; }
    public int NumberOfCars { get; set; }
    public FuelType FuelType { get; set; }

    private string _secretTrainCallsignInField = string.Empty;

    public void SetSecretTrainCallSign(string trainCallsign)
    {
        _secretTrainCallsignInField = trainCallsign;
    }
}

public enum FuelType
{
    Diesel,
    Electric,
    DieselElectric,
    Steam
}
