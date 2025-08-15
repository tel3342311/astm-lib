using AstmLib.Messages;

namespace AstmLib.Tests;

public class HeaderRecordTests
{
    [Fact]
    public void HeaderRecord_Should_Have_Correct_RecordType()
    {
        // Arrange & Act
        var header = new HeaderRecord();
        
        // Assert
        Assert.Equal("H", header.RecordType);
    }
    
    [Fact]
    public void HeaderRecord_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var header = new HeaderRecord();
        
        // Assert
        Assert.Equal(1, header.SequenceNumber);
        Assert.Equal(@"^&\r", header.DelimiterDefinition);
        Assert.Equal("E1394-97", header.VersionNumber);
        Assert.Equal(ProcessingType.Production, header.ProcessingId);
        Assert.NotNull(header.Timestamp);
        Assert.True(header.Timestamp.Value > DateTime.UtcNow.AddMinutes(-1));
    }
    
    [Fact]
    public void HeaderRecord_Validation_Should_Pass_With_Valid_Data()
    {
        // Arrange
        var header = new HeaderRecord
        {
            SenderId = "TestSender",
            ReceiverId = "TestReceiver"
        };
        
        // Act
        var isValid = header.Validate();
        
        // Assert
        Assert.True(isValid);
    }
    
    [Fact]
    public void HeaderRecord_Validation_Should_Fail_With_Invalid_Sequence_Number()
    {
        // Arrange
        var header = new HeaderRecord
        {
            SequenceNumber = -1
        };
        
        // Act
        var result = header.ValidateDetailed();
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message.Contains("Sequence number"));
    }
    
    [Fact]
    public void HeaderRecord_ToAstmString_Should_Generate_Correct_Format()
    {
        // Arrange
        var header = new HeaderRecord
        {
            SequenceNumber = 1,
            MessageControlId = "MSG001",
            SenderId = "LAB001",
            ReceiverId = "LIS001",
            ProcessingId = ProcessingType.Test,
            Timestamp = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc)
        };
        
        // Act
        var astmString = header.ToAstmString();
        
        // Assert
        var fields = astmString.Split('\x1C'); // FS character
        Assert.Equal("H", fields[0]);
        Assert.Equal(@"^&\r", fields[1]);
        Assert.Equal("MSG001", fields[2]);
        Assert.Equal("LAB001", fields[4]);
        Assert.Equal("LIS001", fields[9]);
        Assert.Equal("T", fields[11]);
        Assert.Equal("E1394-97", fields[12]);
        Assert.Equal("20240115103045", fields[13]);
    }
    
    [Fact]
    public void HeaderRecord_Parse_Should_Recreate_Original_Record()
    {
        // Arrange
        var original = new HeaderRecord
        {
            MessageControlId = "MSG001",
            SenderId = "LAB001",
            ReceiverId = "LIS001",
            ProcessingId = ProcessingType.Test,
            Comment = "Test message"
        };
        
        // Act
        var astmString = original.ToAstmString();
        var parsed = HeaderRecord.Parse(astmString);
        
        // Assert
        Assert.Equal(original.MessageControlId, parsed.MessageControlId);
        Assert.Equal(original.SenderId, parsed.SenderId);
        Assert.Equal(original.ReceiverId, parsed.ReceiverId);
        Assert.Equal(original.ProcessingId, parsed.ProcessingId);
        Assert.Equal(original.Comment, parsed.Comment);
        Assert.Equal(original.DelimiterDefinition, parsed.DelimiterDefinition);
        Assert.Equal(original.VersionNumber, parsed.VersionNumber);
    }
    
    [Fact]
    public void HeaderRecord_Clone_Should_Create_Independent_Copy()
    {
        // Arrange
        var original = new HeaderRecord
        {
            SenderId = "Original",
            ReceiverId = "OriginalReceiver"
        };
        
        // Act
        var cloned = (HeaderRecord)original.Clone();
        cloned.SenderId = "Modified";
        
        // Assert
        Assert.Equal("Original", original.SenderId);
        Assert.Equal("Modified", cloned.SenderId);
        Assert.Equal(original.ReceiverId, cloned.ReceiverId);
    }
    
    [Theory]
    [InlineData(ProcessingType.Production, "P")]
    [InlineData(ProcessingType.Test, "T")]
    [InlineData(ProcessingType.Debug, "D")]
    public void HeaderRecord_Should_Serialize_ProcessingType_Correctly(ProcessingType type, string expected)
    {
        // Arrange
        var header = new HeaderRecord { ProcessingId = type };
        
        // Act
        var astmString = header.ToAstmString();
        var fields = astmString.Split('\x1C');
        
        // Assert
        Assert.Equal(expected, fields[11]);
    }
}