using AstmLib.Core;

namespace AstmLib.Messages;

/// <summary>
/// Result Record (R) - ASTM E1394 Section 8.4.4
/// Contains test results and associated information
/// </summary>
public class ResultRecord : AstmMessage
{
    /// <summary>
    /// Record type identifier - always "R" for Result records
    /// </summary>
    public override string RecordType => "R";
    
    /// <summary>
    /// Universal test ID (Field 2) - standardized test identification
    /// Format: test_identifier^test_name^test_type
    /// </summary>
    public string? UniversalTestId { get; set; }
    
    /// <summary>
    /// Data or measurement value (Field 3) - the actual test result
    /// </summary>
    public string? DataValue { get; set; }
    
    /// <summary>
    /// Units (Field 4) - units of measurement for the result
    /// </summary>
    public string? Units { get; set; }
    
    /// <summary>
    /// Reference ranges (Field 5) - normal ranges for the test
    /// </summary>
    public string? ReferenceRanges { get; set; }
    
    /// <summary>
    /// Result abnormal flags (Field 6) - flags indicating abnormal results
    /// </summary>
    public string? AbnormalFlags { get; set; }
    
    /// <summary>
    /// Nature of abnormality testing (Field 7) - type of abnormality testing performed
    /// </summary>
    public string? NatureOfAbnormalityTesting { get; set; }
    
    /// <summary>
    /// Result status (Field 8) - status of the result (Final, Preliminary, etc.)
    /// </summary>
    public string? ResultStatus { get; set; }
    
    /// <summary>
    /// Date of change in normalized values (Field 9) - when reference ranges changed
    /// </summary>
    public DateTime? DateOfChangeInNormalizedValues { get; set; }
    
    /// <summary>
    /// Operator identification (Field 10) - who performed the test
    /// </summary>
    public string? OperatorIdentification { get; set; }
    
    /// <summary>
    /// Date/time test started (Field 11) - when test began
    /// </summary>
    public DateTime? TestStartedDateTime { get; set; }
    
    /// <summary>
    /// Date/time test completed (Field 12) - when test finished
    /// </summary>
    public DateTime? TestCompletedDateTime { get; set; }
    
    /// <summary>
    /// Instrument identification (Field 13) - instrument that performed test
    /// </summary>
    public string? InstrumentIdentification { get; set; }
    
    /// <summary>
    /// Validates Result record specific fields
    /// </summary>
    protected override void ValidateSpecific(List<string> errors)
    {
        // Universal Test ID is required
        if (string.IsNullOrEmpty(UniversalTestId))
        {
            errors.Add("Universal Test ID is required for Result records");
        }
        
        // Data value should be provided
        if (string.IsNullOrEmpty(DataValue))
        {
            errors.Add("Data value should be provided for Result records");
        }
        
        // Validate date consistency
        if (TestStartedDateTime.HasValue && TestCompletedDateTime.HasValue && 
            TestStartedDateTime.Value > TestCompletedDateTime.Value)
        {
            errors.Add("Test started date/time cannot be later than test completed date/time");
        }
    }
    
    /// <summary>
    /// Serializes the Result record to ASTM format string
    /// </summary>
    public override string ToAstmString()
    {
        var dateOfChangeStr = DateOfChangeInNormalizedValues?.ToString("yyyyMMddHHmmss") ?? string.Empty;
        var testStartedStr = TestStartedDateTime?.ToString("yyyyMMddHHmmss") ?? string.Empty;
        var testCompletedStr = TestCompletedDateTime?.ToString("yyyyMMddHHmmss") ?? string.Empty;
        
        return BuildFieldString(
            RecordType,                                        // Field 1: Record Type
            EscapeFieldValue(UniversalTestId),                 // Field 2: Universal Test ID
            EscapeFieldValue(DataValue),                       // Field 3: Data/Measurement Value
            EscapeFieldValue(Units),                           // Field 4: Units
            EscapeFieldValue(ReferenceRanges),                 // Field 5: Reference Ranges
            EscapeFieldValue(AbnormalFlags),                   // Field 6: Result Abnormal Flags
            EscapeFieldValue(NatureOfAbnormalityTesting),      // Field 7: Nature of Abnormality Testing
            EscapeFieldValue(ResultStatus),                    // Field 8: Result Status
            dateOfChangeStr,                                   // Field 9: Date of Change in Normalized Values
            EscapeFieldValue(OperatorIdentification),          // Field 10: Operator Identification
            testStartedStr,                                    // Field 11: Date/Time Test Started
            testCompletedStr,                                  // Field 12: Date/Time Test Completed
            EscapeFieldValue(InstrumentIdentification)         // Field 13: Instrument Identification
        );
    }
    
    /// <summary>
    /// Creates a deep copy of the Result record
    /// </summary>
    public override IAstmMessage Clone()
    {
        return new ResultRecord
        {
            SequenceNumber = SequenceNumber,
            UniversalTestId = UniversalTestId,
            DataValue = DataValue,
            Units = Units,
            ReferenceRanges = ReferenceRanges,
            AbnormalFlags = AbnormalFlags,
            NatureOfAbnormalityTesting = NatureOfAbnormalityTesting,
            ResultStatus = ResultStatus,
            DateOfChangeInNormalizedValues = DateOfChangeInNormalizedValues,
            OperatorIdentification = OperatorIdentification,
            TestStartedDateTime = TestStartedDateTime,
            TestCompletedDateTime = TestCompletedDateTime,
            InstrumentIdentification = InstrumentIdentification
        };
    }
    
    /// <summary>
    /// Parses an ASTM field string into a Result record
    /// </summary>
    public static ResultRecord Parse(string fieldString)
    {
        var fields = ParseFieldString(fieldString);
        var result = new ResultRecord();
        
        if (fields.Length > 1) result.UniversalTestId = UnescapeFieldValue(fields[1]);
        if (fields.Length > 2) result.DataValue = UnescapeFieldValue(fields[2]);
        if (fields.Length > 3) result.Units = UnescapeFieldValue(fields[3]);
        if (fields.Length > 4) result.ReferenceRanges = UnescapeFieldValue(fields[4]);
        if (fields.Length > 5) result.AbnormalFlags = UnescapeFieldValue(fields[5]);
        if (fields.Length > 6) result.NatureOfAbnormalityTesting = UnescapeFieldValue(fields[6]);
        if (fields.Length > 7) result.ResultStatus = UnescapeFieldValue(fields[7]);
        
        if (fields.Length > 8 && !string.IsNullOrEmpty(fields[8]))
        {
            if (DateTime.TryParseExact(fields[8], "yyyyMMddHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var dateOfChange))
            {
                result.DateOfChangeInNormalizedValues = dateOfChange;
            }
        }
        
        if (fields.Length > 9) result.OperatorIdentification = UnescapeFieldValue(fields[9]);
        
        if (fields.Length > 10 && !string.IsNullOrEmpty(fields[10]))
        {
            if (DateTime.TryParseExact(fields[10], "yyyyMMddHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var testStarted))
            {
                result.TestStartedDateTime = testStarted;
            }
        }
        
        if (fields.Length > 11 && !string.IsNullOrEmpty(fields[11]))
        {
            if (DateTime.TryParseExact(fields[11], "yyyyMMddHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var testCompleted))
            {
                result.TestCompletedDateTime = testCompleted;
            }
        }
        
        if (fields.Length > 12) result.InstrumentIdentification = UnescapeFieldValue(fields[12]);
        
        return result;
    }
}