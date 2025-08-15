namespace AstmLib.Core;

/// <summary>
/// Represents the result of validating an ASTM message
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indicates whether the validation passed
    /// </summary>
    public bool IsValid { get; set; }
    
    /// <summary>
    /// Collection of validation errors
    /// </summary>
    public List<ValidationError> Errors { get; set; } = new();
    
    /// <summary>
    /// Collection of validation warnings
    /// </summary>
    public List<ValidationWarning> Warnings { get; set; } = new();
    
    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    /// <returns>Valid validation result</returns>
    public static ValidationResult Success() => new() { IsValid = true };
    
    /// <summary>
    /// Creates a failed validation result with an error
    /// </summary>
    /// <param name="error">The validation error</param>
    /// <returns>Invalid validation result</returns>
    public static ValidationResult Failure(string error) => new()
    {
        IsValid = false,
        Errors = { new ValidationError(error) }
    };
    
    /// <summary>
    /// Creates a failed validation result with multiple errors
    /// </summary>
    /// <param name="errors">Collection of validation errors</param>
    /// <returns>Invalid validation result</returns>
    public static ValidationResult Failure(IEnumerable<string> errors) => new()
    {
        IsValid = false,
        Errors = errors.Select(e => new ValidationError(e)).ToList()
    };
}

/// <summary>
/// Represents a validation error
/// </summary>
public class ValidationError
{
    public string Message { get; set; }
    public string? FieldName { get; set; }
    public string? Code { get; set; }
    
    public ValidationError(string message, string? fieldName = null, string? code = null)
    {
        Message = message;
        FieldName = fieldName;
        Code = code;
    }
}

/// <summary>
/// Represents a validation warning
/// </summary>
public class ValidationWarning
{
    public string Message { get; set; }
    public string? FieldName { get; set; }
    public string? Code { get; set; }
    
    public ValidationWarning(string message, string? fieldName = null, string? code = null)
    {
        Message = message;
        FieldName = fieldName;
        Code = code;
    }
}