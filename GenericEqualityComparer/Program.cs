using GenericEqualityComparer;

namespace GenericEqualizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var toyota1 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };
            var toyota2 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };
            var toyota3 = new Car { Make = "Toyota", Model = "Corolla", Year = 2020 };

            var carEqualityComparer = new GenericEqualityComparer.Lib.GenericEqualityComparer<Car>();

            Console.WriteLine(carEqualityComparer.Equals(toyota1, toyota2)); // True
            Console.WriteLine(carEqualityComparer.Equals(toyota1, toyota3)); // False

            var train1 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.DieselElectric };
            var train2 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.DieselElectric };
            var train3 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.Diesel };

           var trainEqualityComparer = new GenericEqualityComparer.Lib.GenericEqualityComparer<Train>();    

            Console.WriteLine(trainEqualityComparer.Equals(train1, train2)); // True
            Console.WriteLine(trainEqualityComparer.Equals(train1, train3)); // False


        }
    }
}
