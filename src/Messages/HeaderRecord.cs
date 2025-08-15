using AstmLib.Core;

namespace AstmLib.Messages;

/// <summary>
/// Header Record (H) - ASTM E1394 Section 8.4.1
/// Contains sender identification and processing information
/// </summary>
public class HeaderRecord : AstmMessage
{
    /// <summary>
    /// Record type identifier - always "H" for Header records
    /// </summary>
    public override string RecordType => "H";
    
    /// <summary>
    /// Delimiter definition (Field 2) - defines field separators used in the message
    /// Default: ^&amp;\r (field^component&amp;repeat\r)
    /// </summary>
    public string DelimiterDefinition { get; set; } = @"^&\r";
    
    /// <summary>
    /// Message control ID (Field 3) - unique identifier for this message
    /// </summary>
    public string? MessageControlId { get; set; }
    
    /// <summary>
    /// Access password (Field 4) - password for accessing the receiving system
    /// </summary>
    public string? AccessPassword { get; set; }
    
    /// <summary>
    /// Sender name or ID (Field 5) - identification of the sending system
    /// </summary>
    public string? SenderId { get; set; }
    
    /// <summary>
    /// Sender street address (Field 6) - physical address of sender
    /// </summary>
    public string? SenderAddress { get; set; }
    
    /// <summary>
    /// Reserved field (Field 7) - reserved for future use
    /// </summary>
    public string? Reserved { get; set; }
    
    /// <summary>
    /// Sender telephone number (Field 8) - contact phone number
    /// </summary>
    public string? SenderPhone { get; set; }
    
    /// <summary>
    /// Characteristics of sender (Field 9) - capabilities and features
    /// </summary>
    public string? SenderCharacteristics { get; set; }
    
    /// <summary>
    /// Receiver ID (Field 10) - identification of the receiving system
    /// </summary>
    public string? ReceiverId { get; set; }
    
    /// <summary>
    /// Comment or special instructions (Field 11) - additional information
    /// </summary>
    public string? Comment { get; set; }
    
    /// <summary>
    /// Processing ID (Field 12) - indicates type of processing (P=Production, T=Test, D=Debug)
    /// </summary>
    public ProcessingType ProcessingId { get; set; } = ProcessingType.Production;
    
    /// <summary>
    /// Version number (Field 13) - ASTM protocol version
    /// </summary>
    public string VersionNumber { get; set; } = "E1394-97";
    
    /// <summary>
    /// Timestamp (Field 14) - date and time of message creation
    /// </summary>
    public DateTime? Timestamp { get; set; }
    
    /// <summary>
    /// Initializes a new Header record with current timestamp
    /// </summary>
    public HeaderRecord()
    {
        Timestamp = DateTime.UtcNow;
        SequenceNumber = 1; // Header is typically the first record
    }
    
    /// <summary>
    /// Validates Header record specific fields
    /// </summary>
    protected override void ValidateSpecific(List<string> errors)
    {
        if (string.IsNullOrEmpty(DelimiterDefinition))
        {
            errors.Add("Delimiter definition cannot be null or empty");
        }
        else if (DelimiterDefinition.Length < 4)
        {
            errors.Add("Delimiter definition must contain at least 4 characters");
        }
        
        if (string.IsNullOrEmpty(VersionNumber))
        {
            errors.Add("Version number cannot be null or empty");
        }
        
        // Sequence number for Header should typically be 1
        if (SequenceNumber != 1)
        {
            errors.Add("Header record sequence number should typically be 1");
        }
    }
    
    /// <summary>
    /// Serializes the Header record to ASTM format string
    /// </summary>
    public override string ToAstmString()
    {
        var timestampStr = Timestamp?.ToString("yyyyMMddHHmmss") ?? string.Empty;
        var processingIdStr = ProcessingId switch
        {
            ProcessingType.Production => "P",
            ProcessingType.Test => "T",
            ProcessingType.Debug => "D",
            _ => "P"
        };
        
        return BuildFieldString(
            RecordType,                           // Field 1: Record Type
            DelimiterDefinition,                  // Field 2: Delimiter Definition
            EscapeFieldValue(MessageControlId),   // Field 3: Message Control ID
            EscapeFieldValue(AccessPassword),     // Field 4: Access Password
            EscapeFieldValue(SenderId),           // Field 5: Sender Name/ID
            EscapeFieldValue(SenderAddress),      // Field 6: Sender Address
            EscapeFieldValue(Reserved),           // Field 7: Reserved
            EscapeFieldValue(SenderPhone),        // Field 8: Sender Phone
            EscapeFieldValue(SenderCharacteristics), // Field 9: Sender Characteristics
            EscapeFieldValue(ReceiverId),         // Field 10: Receiver ID
            EscapeFieldValue(Comment),            // Field 11: Comment
            processingIdStr,                      // Field 12: Processing ID
            VersionNumber,                        // Field 13: Version Number
            timestampStr                          // Field 14: Timestamp
        );
    }
    
    /// <summary>
    /// Creates a deep copy of the Header record
    /// </summary>
    public override IAstmMessage Clone()
    {
        return new HeaderRecord
        {
            SequenceNumber = SequenceNumber,
            DelimiterDefinition = DelimiterDefinition,
            MessageControlId = MessageControlId,
            AccessPassword = AccessPassword,
            SenderId = SenderId,
            SenderAddress = SenderAddress,
            Reserved = Reserved,
            SenderPhone = SenderPhone,
            SenderCharacteristics = SenderCharacteristics,
            ReceiverId = ReceiverId,
            Comment = Comment,
            ProcessingId = ProcessingId,
            VersionNumber = VersionNumber,
            Timestamp = Timestamp
        };
    }
    
    /// <summary>
    /// Parses an ASTM field string into a Header record
    /// </summary>
    /// <param name="fieldString">ASTM formatted field string</param>
    /// <returns>Parsed Header record</returns>
    public static HeaderRecord Parse(string fieldString)
    {
        var fields = ParseFieldString(fieldString);
        var header = new HeaderRecord();
        
        if (fields.Length > 1) header.DelimiterDefinition = fields[1];
        if (fields.Length > 2) header.MessageControlId = UnescapeFieldValue(fields[2]);
        if (fields.Length > 3) header.AccessPassword = UnescapeFieldValue(fields[3]);
        if (fields.Length > 4) header.SenderId = UnescapeFieldValue(fields[4]);
        if (fields.Length > 5) header.SenderAddress = UnescapeFieldValue(fields[5]);
        if (fields.Length > 6) header.Reserved = UnescapeFieldValue(fields[6]);
        if (fields.Length > 7) header.SenderPhone = UnescapeFieldValue(fields[7]);
        if (fields.Length > 8) header.SenderCharacteristics = UnescapeFieldValue(fields[8]);
        if (fields.Length > 9) header.ReceiverId = UnescapeFieldValue(fields[9]);
        if (fields.Length > 10) header.Comment = UnescapeFieldValue(fields[10]);
        
        if (fields.Length > 11)
        {
            header.ProcessingId = fields[11] switch
            {
                "T" => ProcessingType.Test,
                "D" => ProcessingType.Debug,
                _ => ProcessingType.Production
            };
        }
        
        if (fields.Length > 12) header.VersionNumber = fields[12];
        
        if (fields.Length > 13 && !string.IsNullOrEmpty(fields[13]))
        {
            if (DateTime.TryParseExact(fields[13], "yyyyMMddHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var timestamp))
            {
                header.Timestamp = timestamp;
            }
        }
        
        return header;
    }
}

/// <summary>
/// Processing type enumeration for Header records
/// </summary>
public enum ProcessingType
{
    /// <summary>
    /// Production processing - live data
    /// </summary>
    Production,
    
    /// <summary>
    /// Test processing - test data
    /// </summary>
    Test,
    
    /// <summary>
    /// Debug processing - debugging data
    /// </summary>
    Debug
}