using AstmLib.Validation;

namespace AstmLib.Tests;

public class ChecksumCalculatorTests
{
    [Fact]
    public void StandardChecksumCalculator_Should_Calculate_Correct_Checksum()
    {
        // Arrange
        var calculator = new StandardChecksumCalculator();
        var content = "H|\\^&|||LAB001||||||P|E1394-97|20240101120000";
        
        // Act
        var checksum = calculator.Calculate(content);
        
        // Assert
        Assert.NotNull(checksum);
        Assert.Equal(2, checksum.Length);
        Assert.Matches("^[0-9A-F]{2}$", checksum); // Should be 2-digit hex
    }
    
    [Fact]
    public void StandardChecksumCalculator_Should_Return_00_For_Empty_String()
    {
        // Arrange
        var calculator = new StandardChecksumCalculator();
        
        // Act
        var checksum = calculator.Calculate(string.Empty);
        
        // Assert
        Assert.Equal("00", checksum);
    }
    
    [Fact]
    public void StandardChecksumCalculator_Should_Return_00_For_Null_String()
    {
        // Arrange
        var calculator = new StandardChecksumCalculator();
        
        // Act
        var checksum = calculator.Calculate(null!);
        
        // Assert
        Assert.Equal("00", checksum);
    }
    
    [Theory]
    [InlineData("A", "41")]      // ASCII 'A' = 65 = 0x41
    [InlineData("AB", "83")]     // ASCII 'A' + 'B' = 65 + 66 = 131 = 0x83
    [InlineData("ABC", "C6")]    // ASCII 'A' + 'B' + 'C' = 65 + 66 + 67 = 198 = 0xC6
    public void StandardChecksumCalculator_Should_Calculate_Known_Values(string input, string expected)
    {
        // Arrange
        var calculator = new StandardChecksumCalculator();
        
        // Act
        var checksum = calculator.Calculate(input);
        
        // Assert
        Assert.Equal(expected, checksum);
    }
    
    [Fact]
    public void StandardChecksumCalculator_Should_Handle_Modulo_256()
    {
        // Arrange
        var calculator = new StandardChecksumCalculator();
        var content = new string('A', 300); // 300 * 65 = 19500, 19500 % 256 = 44 = 0x2C
        
        // Act
        var checksum = calculator.Calculate(content);
        
        // Assert
        Assert.Equal("2C", checksum); // Corrected expected value
    }
    
    [Fact]
    public void ChecksumCalculator_Validate_Should_Return_True_For_Correct_Checksum()
    {
        // Arrange
        var calculator = new StandardChecksumCalculator();
        var content = "ABC";
        var correctChecksum = "C6";
        
        // Act
        var isValid = calculator.Validate(content, correctChecksum);
        
        // Assert
        Assert.True(isValid);
    }
    
    [Fact]
    public void ChecksumCalculator_Validate_Should_Return_False_For_Incorrect_Checksum()
    {
        // Arrange
        var calculator = new StandardChecksumCalculator();
        var content = "ABC";
        var incorrectChecksum = "FF";
        
        // Act
        var isValid = calculator.Validate(content, incorrectChecksum);
        
        // Assert
        Assert.False(isValid);
    }
    
    [Fact]
    public void ChecksumCalculator_Validate_Should_Be_Case_Insensitive()
    {
        // Arrange
        var calculator = new StandardChecksumCalculator();
        var content = "ABC";
        var lowercaseChecksum = "c6";
        
        // Act
        var isValid = calculator.Validate(content, lowercaseChecksum);
        
        // Assert
        Assert.True(isValid);
    }
    
    [Theory]
    [InlineData(DeviceType.Standard, typeof(StandardChecksumCalculator))]
    [InlineData(DeviceType.Siemens, typeof(SiemensChecksumCalculator))]
    [InlineData(DeviceType.Abbott, typeof(AbbottChecksumCalculator))]
    [InlineData(DeviceType.Roche, typeof(RocheChecksumCalculator))]
    public void ChecksumCalculator_CreateFor_Should_Return_Correct_Type(DeviceType deviceType, Type expectedType)
    {
        // Act
        var calculator = ChecksumCalculator.CreateFor(deviceType);
        
        // Assert
        Assert.IsType(expectedType, calculator);
    }
    
    [Fact]
    public void DeviceSpecificCalculators_Should_Calculate_Checksum()
    {
        // Arrange
        var content = "TEST";
        var standardCalculator = new StandardChecksumCalculator();
        var siemensCalculator = new SiemensChecksumCalculator();
        var abbottCalculator = new AbbottChecksumCalculator();
        var rocheCalculator = new RocheChecksumCalculator();
        
        // Act
        var standardChecksum = standardCalculator.Calculate(content);
        var siemensChecksum = siemensCalculator.Calculate(content);
        var abbottChecksum = abbottCalculator.Calculate(content);
        var rocheChecksum = rocheCalculator.Calculate(content);
        
        // Assert - Currently all should be the same since they use standard algorithm
        Assert.Equal(standardChecksum, siemensChecksum);
        Assert.Equal(standardChecksum, abbottChecksum);
        Assert.Equal(standardChecksum, rocheChecksum);
        Assert.NotNull(standardChecksum);
        Assert.Equal(2, standardChecksum.Length);
    }
}