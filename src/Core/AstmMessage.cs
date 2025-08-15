using System.Text;

namespace AstmLib.Core;

/// <summary>
/// Abstract base class for all ASTM messages
/// </summary>
public abstract class AstmMessage : IAstmMessage
{
    /// <summary>
    /// The record type identifier (H, P, O, R, C, L, etc.)
    /// </summary>
    public abstract string RecordType { get; }
    
    /// <summary>
    /// Sequence number of the record within the message
    /// </summary>
    public int SequenceNumber { get; set; }
    
    /// <summary>
    /// Validates the message structure and content according to ASTM specifications
    /// </summary>
    /// <returns>True if the message is valid; otherwise false</returns>
    public virtual bool Validate()
    {
        var result = ValidateDetailed();
        return result.IsValid;
    }
    
    /// <summary>
    /// Validates the message and returns detailed validation results
    /// </summary>
    /// <returns>Validation result with details about any errors or warnings</returns>
    public virtual ValidationResult ValidateDetailed()
    {
        var errors = new List<string>();
        
        // Validate sequence number
        if (SequenceNumber < 0 || SequenceNumber > 999999)
        {
            errors.Add("Sequence number must be between 0 and 999999");
        }
        
        // Validate record type
        if (string.IsNullOrEmpty(RecordType))
        {
            errors.Add("Record type cannot be null or empty");
        }
        
        // Allow derived classes to add specific validation
        ValidateSpecific(errors);
        
        return errors.Any() 
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }
    
    /// <summary>
    /// Override this method in derived classes to add specific validation logic
    /// </summary>
    /// <param name="errors">Collection to add validation errors to</param>
    protected virtual void ValidateSpecific(List<string> errors)
    {
        // Default implementation - no additional validation
    }
    
    /// <summary>
    /// Serializes the message to ASTM format string
    /// </summary>
    /// <returns>ASTM formatted string representation</returns>
    public abstract string ToAstmString();
    
    /// <summary>
    /// Creates a deep copy of the message
    /// </summary>
    /// <returns>A new instance with the same data</returns>
    public abstract IAstmMessage Clone();
    
    /// <summary>
    /// Helper method to build ASTM field string with proper separators
    /// </summary>
    /// <param name="fields">Fields to join</param>
    /// <returns>ASTM field string</returns>
    protected static string BuildFieldString(params string?[] fields)
    {
        return string.Join(Utilities.ControlCharacters.FS, 
            fields.Select(f => f ?? string.Empty));
    }
    
    /// <summary>
    /// Helper method to parse ASTM field string
    /// </summary>
    /// <param name="fieldString">ASTM field string to parse</param>
    /// <returns>Array of field values</returns>
    protected static string[] ParseFieldString(string fieldString)
    {
        if (string.IsNullOrEmpty(fieldString))
            return Array.Empty<string>();
            
        return fieldString.Split(Utilities.ControlCharacters.FS[0]);
    }
    
    /// <summary>
    /// Helper method to escape special characters in ASTM fields
    /// </summary>
    /// <param name="value">Value to escape</param>
    /// <returns>Escaped value</returns>
    protected static string EscapeFieldValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
            
        // Escape special characters according to ASTM specifications
        return value
            .Replace("\\", "\\E\\")  // Escape character
            .Replace(Utilities.ControlCharacters.FS, "\\F\\")  // Field separator
            .Replace(Utilities.ControlCharacters.GS, "\\S\\")  // Component separator
            .Replace(Utilities.ControlCharacters.RS, "\\R\\")  // Repeat separator
            .Replace(Utilities.ControlCharacters.CR, "\\X0D\\")  // Carriage return
            .Replace(Utilities.ControlCharacters.LF, "\\X0A\\"); // Line feed
    }
    
    /// <summary>
    /// Helper method to unescape special characters in ASTM fields
    /// </summary>
    /// <param name="value">Value to unescape</param>
    /// <returns>Unescaped value</returns>
    protected static string UnescapeFieldValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
            
        // Unescape special characters according to ASTM specifications
        return value
            .Replace("\\X0A\\", Utilities.ControlCharacters.LF)  // Line feed
            .Replace("\\X0D\\", Utilities.ControlCharacters.CR)  // Carriage return
            .Replace("\\R\\", Utilities.ControlCharacters.RS)    // Repeat separator
            .Replace("\\S\\", Utilities.ControlCharacters.GS)    // Component separator
            .Replace("\\F\\", Utilities.ControlCharacters.FS)    // Field separator
            .Replace("\\E\\", "\\");  // Escape character (must be last)
    }
    
    /// <summary>
    /// Returns a string representation of the message
    /// </summary>
    public override string ToString()
    {
        return $"{RecordType}|{SequenceNumber}";
    }
}