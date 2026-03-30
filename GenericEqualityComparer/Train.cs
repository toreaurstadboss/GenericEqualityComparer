namespace GenericEqualityComparer
{

    public class Train
    {
        public string RouteName{ get; set; }
        public string TrainTypeDesignation { get; set; }
        public int NumberOfCars { get; set; }
        public FuelType FuelType { get; set; }

    }

    public enum FuelType
    {
        Diesel,
        Electric,
        DieselElectric,
        Steam
    }

}
