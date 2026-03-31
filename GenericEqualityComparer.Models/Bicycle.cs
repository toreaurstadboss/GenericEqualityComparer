namespace GenericEqualityComparer.Models;

public class Bicycle
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;

    // Private property - only visible to the comparer when includePrivateProperties: true
    private string FrameSerialNumber { get; set; } = string.Empty;
    public void SetFrameSerialNumber(string serialNumber) => FrameSerialNumber = serialNumber;

    // Private field - only visible to the comparer when includePrivateFields: true
    private string _factoryCode = string.Empty;
    public void SetFactoryCode(string factoryCode) => _factoryCode = factoryCode;
}
