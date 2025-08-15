using AstmLib.Core;

namespace AstmLib.Messages;

/// <summary>
/// Comment Record (C) - ASTM E1394 Section 8.4.5
/// Contains comments and additional information related to patient, order, or result records
/// </summary>
public class CommentRecord : AstmMessage
{
    /// <summary>
    /// Record type identifier - always "C" for Comment records
    /// </summary>
    public override string RecordType => "C";
    
    /// <summary>
    /// Comment source (Field 2) - source of the comment (L=Laboratory, I=Instrument)
    /// </summary>
    public string? CommentSource { get; set; }
    
    /// <summary>
    /// Comment text (Field 3) - the actual comment text
    /// </summary>
    public string? CommentText { get; set; }
    
    /// <summary>
    /// Comment type (Field 4) - type or category of comment
    /// </summary>
    public string? CommentType { get; set; }
    
    /// <summary>
    /// Validates Comment record specific fields
    /// </summary>
    protected override void ValidateSpecific(List<string> errors)
    {
        // Comment text is required
        if (string.IsNullOrEmpty(CommentText))
        {
            errors.Add("Comment text is required for Comment records");
        }
        
        // Comment source validation
        if (!string.IsNullOrEmpty(CommentSource) && 
            !new[] { "L", "I" }.Contains(CommentSource.ToUpper()))
        {
            errors.Add("Comment source must be 'L' (Laboratory) or 'I' (Instrument)");
        }
    }
    
    /// <summary>
    /// Serializes the Comment record to ASTM format string
    /// </summary>
    public override string ToAstmString()
    {
        return BuildFieldString(
            RecordType,                        // Field 1: Record Type
            EscapeFieldValue(CommentSource),   // Field 2: Comment Source
            EscapeFieldValue(CommentText),     // Field 3: Comment Text
            EscapeFieldValue(CommentType)      // Field 4: Comment Type
        );
    }
    
    /// <summary>
    /// Creates a deep copy of the Comment record
    /// </summary>
    public override IAstmMessage Clone()
    {
        return new CommentRecord
        {
            SequenceNumber = SequenceNumber,
            CommentSource = CommentSource,
            CommentText = CommentText,
            CommentType = CommentType
        };
    }
    
    /// <summary>
    /// Parses an ASTM field string into a Comment record
    /// </summary>
    public static CommentRecord Parse(string fieldString)
    {
        var fields = ParseFieldString(fieldString);
        var comment = new CommentRecord();
        
        if (fields.Length > 1) comment.CommentSource = UnescapeFieldValue(fields[1]);
        if (fields.Length > 2) comment.CommentText = UnescapeFieldValue(fields[2]);
        if (fields.Length > 3) comment.CommentType = UnescapeFieldValue(fields[3]);
        
        return comment;
    }
}