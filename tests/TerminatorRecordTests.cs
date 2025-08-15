using AstmLib.Messages;

namespace AstmLib.Tests;

public class TerminatorRecordTests
{
    [Fact]
    public void TerminatorRecord_Should_Have_Correct_RecordType()
    {
        // Arrange & Act
        var terminator = new TerminatorRecord();
        
        // Assert
        Assert.Equal("L", terminator.RecordType);
    }
    
    [Fact]
    public void TerminatorRecord_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var terminator = new TerminatorRecord();
        
        // Assert
        Assert.Equal(1, terminator.SequenceNumber);
        Assert.Equal(TerminationCode.Normal, terminator.TerminationCode);
        Assert.Null(terminator.ErrorDescription);
    }
    
    [Fact]
    public void TerminatorRecord_Validation_Should_Pass_With_Normal_Termination()
    {
        // Arrange
        var terminator = new TerminatorRecord
        {
            TerminationCode = TerminationCode.Normal
        };
        
        // Act
        var isValid = terminator.Validate();
        
        // Assert
        Assert.True(isValid);
    }
    
    [Fact]
    public void TerminatorRecord_Validation_Should_Fail_With_Error_But_No_Description()
    {
        // Arrange
        var terminator = new TerminatorRecord
        {
            TerminationCode = TerminationCode.Error
        };
        
        // Act
        var result = terminator.ValidateDetailed();
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message.Contains("Error description"));
    }
    
    [Fact]
    public void TerminatorRecord_Validation_Should_Pass_With_Error_And_Description()
    {
        // Arrange
        var terminator = new TerminatorRecord
        {
            TerminationCode = TerminationCode.Error,
            ErrorDescription = "Communication timeout"
        };
        
        // Act
        var isValid = terminator.Validate();
        
        // Assert
        Assert.True(isValid);
    }
    
    [Fact]
    public void TerminatorRecord_ToAstmString_Should_Generate_Correct_Format()
    {
        // Arrange
        var terminator = new TerminatorRecord
        {
            TerminationCode = TerminationCode.Error,
            ErrorDescription = "Test error"
        };
        
        // Act
        var astmString = terminator.ToAstmString();
        
        // Assert
        var fields = astmString.Split('\x1C'); // FS character
        Assert.Equal("L", fields[0]);
        Assert.Equal("E", fields[1]);
        Assert.Equal("Test error", fields[2]);
    }
    
    [Fact]
    public void TerminatorRecord_Parse_Should_Recreate_Original_Record()
    {
        // Arrange
        var original = new TerminatorRecord
        {
            TerminationCode = TerminationCode.Final,
            ErrorDescription = "Final transmission"
        };
        
        // Act
        var astmString = original.ToAstmString();
        var parsed = TerminatorRecord.Parse(astmString);
        
        // Assert
        Assert.Equal(original.TerminationCode, parsed.TerminationCode);
        Assert.Equal(original.ErrorDescription, parsed.ErrorDescription);
    }
    
    [Fact]
    public void TerminatorRecord_Clone_Should_Create_Independent_Copy()
    {
        // Arrange
        var original = new TerminatorRecord
        {
            TerminationCode = TerminationCode.Error,
            ErrorDescription = "Original error"
        };
        
        // Act
        var cloned = (TerminatorRecord)original.Clone();
        cloned.ErrorDescription = "Modified error";
        
        // Assert
        Assert.Equal("Original error", original.ErrorDescription);
        Assert.Equal("Modified error", cloned.ErrorDescription);
        Assert.Equal(original.TerminationCode, cloned.TerminationCode);
    }
    
    [Theory]
    [InlineData(TerminationCode.Normal, "N")]
    [InlineData(TerminationCode.Final, "F")]
    [InlineData(TerminationCode.Error, "E")]
    public void TerminatorRecord_Should_Serialize_TerminationCode_Correctly(TerminationCode code, string expected)
    {
        // Arrange
        var terminator = new TerminatorRecord 
        { 
            TerminationCode = code,
            ErrorDescription = code == TerminationCode.Error ? "Test error" : null
        };
        
        // Act
        var astmString = terminator.ToAstmString();
        var fields = astmString.Split('\x1C');
        
        // Assert
        Assert.Equal(expected, fields[1]);
    }
    
    [Fact]
    public void CreateNormal_Should_Return_Normal_Terminator()
    {
        // Arrange & Act
        var terminator = TerminatorRecord.CreateNormal();
        
        // Assert
        Assert.Equal(TerminationCode.Normal, terminator.TerminationCode);
        Assert.Null(terminator.ErrorDescription);
    }
    
    [Fact]
    public void CreateFinal_Should_Return_Final_Terminator()
    {
        // Arrange & Act
        var terminator = TerminatorRecord.CreateFinal();
        
        // Assert
        Assert.Equal(TerminationCode.Final, terminator.TerminationCode);
        Assert.Null(terminator.ErrorDescription);
    }
    
    [Fact]
    public void CreateError_Should_Return_Error_Terminator_With_Description()
    {
        // Arrange
        var errorDescription = "Communication failed";
        
        // Act
        var terminator = TerminatorRecord.CreateError(errorDescription);
        
        // Assert
        Assert.Equal(TerminationCode.Error, terminator.TerminationCode);
        Assert.Equal(errorDescription, terminator.ErrorDescription);
    }
}