using GenericEqualityComparer.Models;

namespace GenericEqualizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // -------------------------------------------------------
            // Public properties — Car
            // -------------------------------------------------------
            var toyota1 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };
            var toyota2 = new Car { Make = "Toyota", Model = "Camry", Year = 2020 };
            var toyota3 = new Car { Make = "Toyota", Model = "Corolla", Year = 2020 };

            var carComparer = new GenericEqualityComparer.Lib.GenericEqualityComparer<Car>();

            Console.WriteLine("--- Car: public properties ---");
            Console.WriteLine($"toyota1 equals toyota2: {carComparer.Equals(toyota1, toyota2)}"); // True
            Console.WriteLine($"toyota1 equals toyota3: {carComparer.Equals(toyota1, toyota3)}"); // False

            // -------------------------------------------------------
            // Public properties — Train (with enum field)
            // -------------------------------------------------------
            var train1 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.DieselElectric };
            var train2 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.DieselElectric };
            var train3 = new Train { RouteName = "R7", TrainTypeDesignation = "Type76", NumberOfCars = 6, FuelType = FuelType.Diesel };

            var trainComparer = new GenericEqualityComparer.Lib.GenericEqualityComparer<Train>();

            Console.WriteLine("\n--- Train: public properties ---");
            Console.WriteLine($"train1 equals train2: {trainComparer.Equals(train1, train2)}"); // True
            Console.WriteLine($"train1 equals train3: {trainComparer.Equals(train1, train3)}"); // False

            // -------------------------------------------------------
            // Private property — Bicycle
            // -------------------------------------------------------
            // bike1 and bike2 share the same serial; bike3 has a different one
            var bike1 = new Bicycle { Brand = "Trek", Model = "FX3" };
            var bike2 = new Bicycle { Brand = "Trek", Model = "FX3" };
            var bike3 = new Bicycle { Brand = "Trek", Model = "FX3" };
            bike1.SetFrameSerialNumber("SN-001");
            bike2.SetFrameSerialNumber("SN-001");
            bike3.SetFrameSerialNumber("SN-999"); // intentionally different

            var defaultBikeComparer = new GenericEqualityComparer.Lib.GenericEqualityComparer<Bicycle>();
            var privatePropsBikeComparer = new GenericEqualityComparer.Lib.GenericEqualityComparer<Bicycle>(includePrivateProperties: true);

            Console.WriteLine("\n--- Bicycle: private property (FrameSerialNumber) ---");
            Console.WriteLine($"bike1 equals bike2 (default):                    {defaultBikeComparer.Equals(bike1, bike2)}"); // True  - same serial, but ignored
            Console.WriteLine($"bike1 equals bike3 (default):                    {defaultBikeComparer.Equals(bike1, bike3)}"); // True  - different serial, but ignored
            Console.WriteLine($"bike1 equals bike2 (includePrivateProperties):   {privatePropsBikeComparer.Equals(bike1, bike2)}"); // True  - same serial
            Console.WriteLine($"bike1 equals bike3 (includePrivateProperties):   {privatePropsBikeComparer.Equals(bike1, bike3)}"); // False - different serial detected

            // -------------------------------------------------------
            // Private field — Bicycle
            // -------------------------------------------------------
            // bike4 and bike5 have the same factory code; bike6 differs
            var bike4 = new Bicycle { Brand = "Giant", Model = "Escape" };
            var bike5 = new Bicycle { Brand = "Giant", Model = "Escape" };
            var bike6 = new Bicycle { Brand = "Giant", Model = "Escape" };
            bike4.SetFactoryCode("FACTORY-A");
            bike5.SetFactoryCode("FACTORY-A");
            bike6.SetFactoryCode("FACTORY-B"); // intentionally different

            var defaultBikeComparer2 = new GenericEqualityComparer.Lib.GenericEqualityComparer<Bicycle>();
            var privateFieldsBikeComparer = new GenericEqualityComparer.Lib.GenericEqualityComparer<Bicycle>(includePrivateFields: true);

            Console.WriteLine("\n--- Bicycle: private field (_factoryCode) ---");
            Console.WriteLine($"bike4 equals bike5 (default):               {defaultBikeComparer2.Equals(bike4, bike5)}"); // True  - same code, but ignored
            Console.WriteLine($"bike4 equals bike6 (default):               {defaultBikeComparer2.Equals(bike4, bike6)}"); // True  - different code, but ignored
            Console.WriteLine($"bike4 equals bike5 (includePrivateFields):  {privateFieldsBikeComparer.Equals(bike4, bike5)}"); // True  - same code
            Console.WriteLine($"bike4 equals bike6 (includePrivateFields):  {privateFieldsBikeComparer.Equals(bike4, bike6)}"); // False - different code detected

            // -------------------------------------------------------
            // Private field — Car (existing _secretAssemblyNumberInField)
            // -------------------------------------------------------
            var ford1 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
            var ford2 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
            var ford3 = new Car { Make = "Ford", Model = "Focus", Year = 2022 };
            ford1.SetSecretAssemblyNumber("ASM-001");
            ford2.SetSecretAssemblyNumber("ASM-001");
            ford3.SetSecretAssemblyNumber("ASM-999"); // intentionally different

            var defaultCarComparer = new GenericEqualityComparer.Lib.GenericEqualityComparer<Car>();
            var privateFieldsCarComparer = new GenericEqualityComparer.Lib.GenericEqualityComparer<Car>(includePrivateFields: true);

            Console.WriteLine("\n--- Car: private field (_secretAssemblyNumberInField) ---");
            Console.WriteLine($"ford1 equals ford2 (default):               {defaultCarComparer.Equals(ford1, ford2)}"); // True  - same, but ignored
            Console.WriteLine($"ford1 equals ford3 (default):               {defaultCarComparer.Equals(ford1, ford3)}"); // True  - different, but ignored
            Console.WriteLine($"ford1 equals ford2 (includePrivateFields):  {privateFieldsCarComparer.Equals(ford1, ford2)}"); // True  - same
            Console.WriteLine($"ford1 equals ford3 (includePrivateFields):  {privateFieldsCarComparer.Equals(ford1, ford3)}"); // False - different detected
        }
    }
}
