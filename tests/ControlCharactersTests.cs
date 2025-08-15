using AstmLib.Utilities;

namespace AstmLib.Tests;

public class ControlCharactersTests
{
    [Fact]
    public void ControlCharacters_Should_Have_Correct_ASCII_Values()
    {
        // Arrange & Act & Assert
        Assert.Equal(5, Convert.ToInt32(ControlCharacters.ENQ[0]));
        Assert.Equal(6, Convert.ToInt32(ControlCharacters.ACK[0]));
        Assert.Equal(21, Convert.ToInt32(ControlCharacters.NAK[0]));
        Assert.Equal(2, Convert.ToInt32(ControlCharacters.STX[0]));
        Assert.Equal(3, Convert.ToInt32(ControlCharacters.ETX[0]));
        Assert.Equal(4, Convert.ToInt32(ControlCharacters.EOT[0]));
        Assert.Equal(10, Convert.ToInt32(ControlCharacters.LF[0]));
        Assert.Equal(13, Convert.ToInt32(ControlCharacters.CR[0]));
        Assert.Equal(23, Convert.ToInt32(ControlCharacters.ETB[0]));
        Assert.Equal(28, Convert.ToInt32(ControlCharacters.FS[0]));
        Assert.Equal(29, Convert.ToInt32(ControlCharacters.GS[0]));
        Assert.Equal(30, Convert.ToInt32(ControlCharacters.RS[0]));
        Assert.Equal(31, Convert.ToInt32(ControlCharacters.US[0]));
    }
    
    [Fact]
    public void CRLF_Should_Be_Combination_Of_CR_And_LF()
    {
        // Arrange & Act
        var expected = ControlCharacters.CR + ControlCharacters.LF;
        
        // Assert
        Assert.Equal(expected, ControlCharacters.CRLF);
    }
    
    [Fact]
    public void Constants_Should_Have_Expected_Values()
    {
        // Arrange & Act & Assert
        Assert.Equal(15000, ControlCharacters.DefaultTimeoutMs);
        Assert.Equal(7, ControlCharacters.MaxFrameSequence);
    }
}