namespace AstmLib.Validation;

/// <summary>
/// Abstract base class for calculating ASTM message checksums
/// Different devices may use different checksum algorithms
/// </summary>
public abstract class ChecksumCalculator
{
    /// <summary>
    /// Calculates the checksum for the given message content
    /// </summary>
    /// <param name="content">Message content to calculate checksum for</param>
    /// <returns>Calculated checksum as hexadecimal string</returns>
    public abstract string Calculate(string content);
    
    /// <summary>
    /// Validates that the provided checksum matches the calculated one
    /// </summary>
    /// <param name="content">Message content</param>
    /// <param name="providedChecksum">Checksum to validate</param>
    /// <returns>True if checksum is valid</returns>
    public virtual bool Validate(string content, string providedChecksum)
    {
        var calculatedChecksum = Calculate(content);
        return string.Equals(calculatedChecksum, providedChecksum, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Factory method to create checksum calculator for specific device types
    /// </summary>
    /// <param name="deviceType">Type of device</param>
    /// <returns>Appropriate checksum calculator</returns>
    public static ChecksumCalculator CreateFor(DeviceType deviceType)
    {
        return deviceType switch
        {
            DeviceType.Standard => new StandardChecksumCalculator(),
            DeviceType.Siemens => new SiemensChecksumCalculator(),
            DeviceType.Abbott => new AbbottChecksumCalculator(),
            DeviceType.Roche => new RocheChecksumCalculator(),
            _ => new StandardChecksumCalculator()
        };
    }
}

/// <summary>
/// Standard ASTM E1394 checksum calculator
/// Uses modulo 256 sum of all bytes
/// </summary>
public class StandardChecksumCalculator : ChecksumCalculator
{
    /// <summary>
    /// Calculates standard ASTM checksum (sum of bytes modulo 256)
    /// </summary>
    /// <param name="content">Message content</param>
    /// <returns>Checksum as 2-digit uppercase hexadecimal</returns>
    public override string Calculate(string content)
    {
        if (string.IsNullOrEmpty(content))
            return "00";
        
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var sum = 0;
        
        foreach (var b in bytes)
        {
            sum += b;
        }
        
        var checksum = sum % 256;
        return checksum.ToString("X2");
    }
}

/// <summary>
/// Siemens-specific checksum calculator
/// May have device-specific variations
/// </summary>
public class SiemensChecksumCalculator : ChecksumCalculator
{
    public override string Calculate(string content)
    {
        // For now, use standard algorithm
        // Can be customized based on Siemens device requirements
        return new StandardChecksumCalculator().Calculate(content);
    }
}

/// <summary>
/// Abbott-specific checksum calculator
/// May have device-specific variations
/// </summary>
public class AbbottChecksumCalculator : ChecksumCalculator
{
    public override string Calculate(string content)
    {
        // For now, use standard algorithm
        // Can be customized based on Abbott device requirements
        return new StandardChecksumCalculator().Calculate(content);
    }
}

/// <summary>
/// Roche-specific checksum calculator
/// May have device-specific variations
/// </summary>
public class RocheChecksumCalculator : ChecksumCalculator
{
    public override string Calculate(string content)
    {
        // For now, use standard algorithm
        // Can be customized based on Roche device requirements
        return new StandardChecksumCalculator().Calculate(content);
    }
}

/// <summary>
/// Device type enumeration for checksum calculation
/// </summary>
public enum DeviceType
{
    /// <summary>
    /// Standard ASTM E1394 device
    /// </summary>
    Standard,
    
    /// <summary>
    /// Siemens laboratory instruments
    /// </summary>
    Siemens,
    
    /// <summary>
    /// Abbott laboratory instruments
    /// </summary>
    Abbott,
    
    /// <summary>
    /// Roche laboratory instruments
    /// </summary>
    Roche,
    
    /// <summary>
    /// Beckman Coulter instruments
    /// </summary>
    BeckmanCoulter,
    
    /// <summary>
    /// Bio-Rad instruments
    /// </summary>
    BioRad,
    
    /// <summary>
    /// Generic/custom device
    /// </summary>
    Custom
}