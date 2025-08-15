using AstmLib.Core;

namespace AstmLib.Messages;

/// <summary>
/// Terminator Record (L) - ASTM E1394 Section 8.4.7
/// Marks the end of an ASTM message transmission
/// </summary>
public class TerminatorRecord : AstmMessage
{
    /// <summary>
    /// Record type identifier - always "L" for Terminator records
    /// </summary>
    public override string RecordType => "L";
    
    /// <summary>
    /// Termination code (Field 2) - indicates normal or abnormal termination
    /// N = Normal termination, F = Final termination, E = Error termination
    /// </summary>
    public TerminationCode TerminationCode { get; set; } = TerminationCode.Normal;
    
    /// <summary>
    /// Error description (Field 3) - describes error if termination code is E
    /// </summary>
    public string? ErrorDescription { get; set; }
    
    /// <summary>
    /// Initializes a new Terminator record
    /// </summary>
    public TerminatorRecord()
    {
        SequenceNumber = 1; // Terminator typically has sequence number 1
    }
    
    /// <summary>
    /// Validates Terminator record specific fields
    /// </summary>
    protected override void ValidateSpecific(List<string> errors)
    {
        // If termination code is Error, error description should be provided
        if (TerminationCode == TerminationCode.Error && string.IsNullOrEmpty(ErrorDescription))
        {
            errors.Add("Error description must be provided when termination code is Error");
        }
    }
    
    /// <summary>
    /// Serializes the Terminator record to ASTM format string
    /// </summary>
    public override string ToAstmString()
    {
        var terminationCodeStr = TerminationCode switch
        {
            TerminationCode.Normal => "N",
            TerminationCode.Final => "F",
            TerminationCode.Error => "E",
            _ => "N"
        };
        
        return BuildFieldString(
            RecordType,                         // Field 1: Record Type
            terminationCodeStr,                 // Field 2: Termination Code
            EscapeFieldValue(ErrorDescription)  // Field 3: Error Description
        );
    }
    
    /// <summary>
    /// Creates a deep copy of the Terminator record
    /// </summary>
    public override IAstmMessage Clone()
    {
        return new TerminatorRecord
        {
            SequenceNumber = SequenceNumber,
            TerminationCode = TerminationCode,
            ErrorDescription = ErrorDescription
        };
    }
    
    /// <summary>
    /// Parses an ASTM field string into a Terminator record
    /// </summary>
    /// <param name="fieldString">ASTM formatted field string</param>
    /// <returns>Parsed Terminator record</returns>
    public static TerminatorRecord Parse(string fieldString)
    {
        var fields = ParseFieldString(fieldString);
        var terminator = new TerminatorRecord();
        
        if (fields.Length > 1)
        {
            terminator.TerminationCode = fields[1] switch
            {
                "F" => TerminationCode.Final,
                "E" => TerminationCode.Error,
                _ => TerminationCode.Normal
            };
        }
        
        if (fields.Length > 2)
        {
            terminator.ErrorDescription = UnescapeFieldValue(fields[2]);
        }
        
        return terminator;
    }
    
    /// <summary>
    /// Creates a normal termination record
    /// </summary>
    /// <returns>Terminator record with normal termination</returns>
    public static TerminatorRecord CreateNormal()
    {
        return new TerminatorRecord { TerminationCode = TerminationCode.Normal };
    }
    
    /// <summary>
    /// Creates a final termination record
    /// </summary>
    /// <returns>Terminator record with final termination</returns>
    public static TerminatorRecord CreateFinal()
    {
        return new TerminatorRecord { TerminationCode = TerminationCode.Final };
    }
    
    /// <summary>
    /// Creates an error termination record
    /// </summary>
    /// <param name="errorDescription">Description of the error</param>
    /// <returns>Terminator record with error termination</returns>
    public static TerminatorRecord CreateError(string errorDescription)
    {
        return new TerminatorRecord 
        { 
            TerminationCode = TerminationCode.Error,
            ErrorDescription = errorDescription
        };
    }
}

/// <summary>
/// Termination code enumeration for Terminator records
/// </summary>
public enum TerminationCode
{
    /// <summary>
    /// Normal termination - transmission completed successfully
    /// </summary>
    Normal,
    
    /// <summary>
    /// Final termination - final transmission in a sequence
    /// </summary>
    Final,
    
    /// <summary>
    /// Error termination - transmission ended due to error
    /// </summary>
    Error
}