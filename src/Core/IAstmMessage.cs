namespace AstmLib.Core;

/// <summary>
/// Represents an ASTM message that can be transmitted between clinical laboratory instruments and computer systems.
/// </summary>
public interface IAstmMessage
{
    /// <summary>
    /// The record type identifier (H, P, O, R, C, L, etc.)
    /// </summary>
    string RecordType { get; }
    
    /// <summary>
    /// Sequence number of the record within the message
    /// </summary>
    int SequenceNumber { get; set; }
    
    /// <summary>
    /// Validates the message structure and content according to ASTM specifications
    /// </summary>
    /// <returns>True if the message is valid; otherwise false</returns>
    bool Validate();
    
    /// <summary>
    /// Validates the message and returns detailed validation results
    /// </summary>
    /// <returns>Validation result with details about any errors or warnings</returns>
    ValidationResult ValidateDetailed();
    
    /// <summary>
    /// Serializes the message to ASTM format string
    /// </summary>
    /// <returns>ASTM formatted string representation</returns>
    string ToAstmString();
    
    /// <summary>
    /// Creates a deep copy of the message
    /// </summary>
    /// <returns>A new instance with the same data</returns>
    IAstmMessage Clone();
}