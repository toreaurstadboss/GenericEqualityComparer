namespace GenericEqualityComparer.Models;

public class Car
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }

    public void SetSecretAssemblyNumber(string assemblyNumber)
    {
        _secretAssemblyNumberInField = assemblyNumber;
    }

    private string _secretAssemblyNumberInField = string.Empty;
}
