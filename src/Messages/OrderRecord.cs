using AstmLib.Core;

namespace AstmLib.Messages;

/// <summary>
/// Order Record (O) - ASTM E1394 Section 8.4.3
/// Contains test order information and specimen details
/// </summary>
public class OrderRecord : AstmMessage
{
    /// <summary>
    /// Record type identifier - always "O" for Order records
    /// </summary>
    public override string RecordType => "O";
    
    /// <summary>
    /// Specimen ID (Field 2) - unique identifier for the specimen
    /// </summary>
    public string? SpecimenId { get; set; }
    
    /// <summary>
    /// Instrument specimen ID (Field 3) - instrument-assigned specimen identifier
    /// </summary>
    public string? InstrumentSpecimenId { get; set; }
    
    /// <summary>
    /// Universal test ID (Field 4) - standardized test identification
    /// Format: test_identifier^test_name^test_type
    /// </summary>
    public string? UniversalTestId { get; set; }
    
    /// <summary>
    /// Priority (Field 5) - test priority level
    /// </summary>
    public string? Priority { get; set; }
    
    /// <summary>
    /// Requested/ordered date and time (Field 6) - when test was ordered
    /// </summary>
    public DateTime? RequestedDateTime { get; set; }
    
    /// <summary>
    /// Specimen collection date and time (Field 7) - when specimen was collected
    /// </summary>
    public DateTime? CollectionDateTime { get; set; }
    
    /// <summary>
    /// Collector ID (Field 8) - identifier of person who collected specimen
    /// </summary>
    public string? CollectorId { get; set; }
    
    /// <summary>
    /// Action code (Field 9) - action to be taken with the test
    /// </summary>
    public string? ActionCode { get; set; }
    
    /// <summary>
    /// Danger code (Field 10) - any danger associated with specimen
    /// </summary>
    public string? DangerCode { get; set; }
    
    /// <summary>
    /// Relevant clinical information (Field 11) - clinical context for test
    /// </summary>
    public string? ClinicalInformation { get; set; }
    
    /// <summary>
    /// Date/time specimen received (Field 12) - when lab received specimen
    /// </summary>
    public DateTime? ReceivedDateTime { get; set; }
    
    /// <summary>
    /// Specimen descriptor (Field 13) - description of specimen type
    /// </summary>
    public string? SpecimenDescriptor { get; set; }
    
    /// <summary>
    /// Ordering physician (Field 14) - physician who ordered the test
    /// </summary>
    public string? OrderingPhysician { get; set; }
    
    /// <summary>
    /// Physician's phone number (Field 15) - contact information for physician
    /// </summary>
    public string? PhysicianPhone { get; set; }
    
    /// <summary>
    /// User field 1 (Field 16) - user-defined field
    /// </summary>
    public string? UserField1 { get; set; }
    
    /// <summary>
    /// User field 2 (Field 17) - user-defined field
    /// </summary>
    public string? UserField2 { get; set; }
    
    /// <summary>
    /// Laboratory field 1 (Field 18) - laboratory-defined field
    /// </summary>
    public string? LaboratoryField1 { get; set; }
    
    /// <summary>
    /// Laboratory field 2 (Field 19) - laboratory-defined field
    /// </summary>
    public string? LaboratoryField2 { get; set; }
    
    /// <summary>
    /// Date/time results reported or last modified (Field 20) - reporting timestamp
    /// </summary>
    public DateTime? ReportedDateTime { get; set; }
    
    /// <summary>
    /// Instrument charge to computer system (Field 21) - billing information
    /// </summary>
    public string? InstrumentCharge { get; set; }
    
    /// <summary>
    /// Instrument section ID (Field 22) - section where test is performed
    /// </summary>
    public string? InstrumentSectionId { get; set; }
    
    /// <summary>
    /// Report types (Field 23) - types of reports to generate
    /// </summary>
    public string? ReportTypes { get; set; }
    
    /// <summary>
    /// Reserved field (Field 24) - reserved for future use
    /// </summary>
    public string? Reserved { get; set; }
    
    /// <summary>
    /// Location of specimen collection (Field 25) - where specimen was collected
    /// </summary>
    public string? CollectionLocation { get; set; }
    
    /// <summary>
    /// Nosocomial infection flag (Field 26) - hospital-acquired infection indicator
    /// </summary>
    public string? NosocomialInfectionFlag { get; set; }
    
    /// <summary>
    /// Specimen service (Field 27) - service associated with specimen
    /// </summary>
    public string? SpecimenService { get; set; }
    
    /// <summary>
    /// Specimen institution (Field 28) - institution where specimen originated
    /// </summary>
    public string? SpecimenInstitution { get; set; }
    
    /// <summary>
    /// Validates Order record specific fields
    /// </summary>
    protected override void ValidateSpecific(List<string> errors)
    {
        // Specimen ID is required
        if (string.IsNullOrEmpty(SpecimenId))
        {
            errors.Add("Specimen ID is required for Order records");
        }
        
        // Universal Test ID should be provided
        if (string.IsNullOrEmpty(UniversalTestId))
        {
            errors.Add("Universal Test ID should be provided for Order records");
        }
        
        // Validate date consistency
        if (RequestedDateTime.HasValue && CollectionDateTime.HasValue && 
            RequestedDateTime.Value > CollectionDateTime.Value)
        {
            errors.Add("Requested date/time cannot be later than collection date/time");
        }
        
        if (CollectionDateTime.HasValue && ReceivedDateTime.HasValue && 
            CollectionDateTime.Value > ReceivedDateTime.Value)
        {
            errors.Add("Collection date/time cannot be later than received date/time");
        }
    }
    
    /// <summary>
    /// Serializes the Order record to ASTM format string
    /// </summary>
    public override string ToAstmString()
    {
        var requestedDateTimeStr = RequestedDateTime?.ToString("yyyyMMddHHmmss") ?? string.Empty;
        var collectionDateTimeStr = CollectionDateTime?.ToString("yyyyMMddHHmmss") ?? string.Empty;
        var receivedDateTimeStr = ReceivedDateTime?.ToString("yyyyMMddHHmmss") ?? string.Empty;
        var reportedDateTimeStr = ReportedDateTime?.ToString("yyyyMMddHHmmss") ?? string.Empty;
        
        return BuildFieldString(
            RecordType,                                    // Field 1: Record Type
            EscapeFieldValue(SpecimenId),                  // Field 2: Specimen ID
            EscapeFieldValue(InstrumentSpecimenId),        // Field 3: Instrument Specimen ID
            EscapeFieldValue(UniversalTestId),             // Field 4: Universal Test ID
            EscapeFieldValue(Priority),                    // Field 5: Priority
            requestedDateTimeStr,                          // Field 6: Requested/Ordered Date Time
            collectionDateTimeStr,                         // Field 7: Specimen Collection Date Time
            EscapeFieldValue(CollectorId),                 // Field 8: Collector ID
            EscapeFieldValue(ActionCode),                  // Field 9: Action Code
            EscapeFieldValue(DangerCode),                  // Field 10: Danger Code
            EscapeFieldValue(ClinicalInformation),         // Field 11: Clinical Information
            receivedDateTimeStr,                           // Field 12: Date/Time Specimen Received
            EscapeFieldValue(SpecimenDescriptor),          // Field 13: Specimen Descriptor
            EscapeFieldValue(OrderingPhysician),           // Field 14: Ordering Physician
            EscapeFieldValue(PhysicianPhone),              // Field 15: Physician's Phone
            EscapeFieldValue(UserField1),                  // Field 16: User Field 1
            EscapeFieldValue(UserField2),                  // Field 17: User Field 2
            EscapeFieldValue(LaboratoryField1),            // Field 18: Laboratory Field 1
            EscapeFieldValue(LaboratoryField2),            // Field 19: Laboratory Field 2
            reportedDateTimeStr,                           // Field 20: Date/Time Results Reported
            EscapeFieldValue(InstrumentCharge),            // Field 21: Instrument Charge
            EscapeFieldValue(InstrumentSectionId),         // Field 22: Instrument Section ID
            EscapeFieldValue(ReportTypes),                 // Field 23: Report Types
            EscapeFieldValue(Reserved),                    // Field 24: Reserved
            EscapeFieldValue(CollectionLocation),          // Field 25: Location of Specimen Collection
            EscapeFieldValue(NosocomialInfectionFlag),     // Field 26: Nosocomial Infection Flag
            EscapeFieldValue(SpecimenService),             // Field 27: Specimen Service
            EscapeFieldValue(SpecimenInstitution)          // Field 28: Specimen Institution
        );
    }
    
    /// <summary>
    /// Creates a deep copy of the Order record
    /// </summary>
    public override IAstmMessage Clone()
    {
        return new OrderRecord
        {
            SequenceNumber = SequenceNumber,
            SpecimenId = SpecimenId,
            InstrumentSpecimenId = InstrumentSpecimenId,
            UniversalTestId = UniversalTestId,
            Priority = Priority,
            RequestedDateTime = RequestedDateTime,
            CollectionDateTime = CollectionDateTime,
            CollectorId = CollectorId,
            ActionCode = ActionCode,
            DangerCode = DangerCode,
            ClinicalInformation = ClinicalInformation,
            ReceivedDateTime = ReceivedDateTime,
            SpecimenDescriptor = SpecimenDescriptor,
            OrderingPhysician = OrderingPhysician,
            PhysicianPhone = PhysicianPhone,
            UserField1 = UserField1,
            UserField2 = UserField2,
            LaboratoryField1 = LaboratoryField1,
            LaboratoryField2 = LaboratoryField2,
            ReportedDateTime = ReportedDateTime,
            InstrumentCharge = InstrumentCharge,
            InstrumentSectionId = InstrumentSectionId,
            ReportTypes = ReportTypes,
            Reserved = Reserved,
            CollectionLocation = CollectionLocation,
            NosocomialInfectionFlag = NosocomialInfectionFlag,
            SpecimenService = SpecimenService,
            SpecimenInstitution = SpecimenInstitution
        };
    }
    
    /// <summary>
    /// Parses an ASTM field string into an Order record
    /// </summary>
    public static OrderRecord Parse(string fieldString)
    {
        var fields = ParseFieldString(fieldString);
        var order = new OrderRecord();
        
        if (fields.Length > 1) order.SpecimenId = UnescapeFieldValue(fields[1]);
        if (fields.Length > 2) order.InstrumentSpecimenId = UnescapeFieldValue(fields[2]);
        if (fields.Length > 3) order.UniversalTestId = UnescapeFieldValue(fields[3]);
        if (fields.Length > 4) order.Priority = UnescapeFieldValue(fields[4]);
        
        if (fields.Length > 5 && !string.IsNullOrEmpty(fields[5]))
        {
            if (DateTime.TryParseExact(fields[5], "yyyyMMddHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var requestedDateTime))
            {
                order.RequestedDateTime = requestedDateTime;
            }
        }
        
        if (fields.Length > 6 && !string.IsNullOrEmpty(fields[6]))
        {
            if (DateTime.TryParseExact(fields[6], "yyyyMMddHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var collectionDateTime))
            {
                order.CollectionDateTime = collectionDateTime;
            }
        }
        
        if (fields.Length > 7) order.CollectorId = UnescapeFieldValue(fields[7]);
        if (fields.Length > 8) order.ActionCode = UnescapeFieldValue(fields[8]);
        if (fields.Length > 9) order.DangerCode = UnescapeFieldValue(fields[9]);
        if (fields.Length > 10) order.ClinicalInformation = UnescapeFieldValue(fields[10]);
        
        if (fields.Length > 11 && !string.IsNullOrEmpty(fields[11]))
        {
            if (DateTime.TryParseExact(fields[11], "yyyyMMddHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var receivedDateTime))
            {
                order.ReceivedDateTime = receivedDateTime;
            }
        }
        
        if (fields.Length > 12) order.SpecimenDescriptor = UnescapeFieldValue(fields[12]);
        if (fields.Length > 13) order.OrderingPhysician = UnescapeFieldValue(fields[13]);
        if (fields.Length > 14) order.PhysicianPhone = UnescapeFieldValue(fields[14]);
        if (fields.Length > 15) order.UserField1 = UnescapeFieldValue(fields[15]);
        if (fields.Length > 16) order.UserField2 = UnescapeFieldValue(fields[16]);
        if (fields.Length > 17) order.LaboratoryField1 = UnescapeFieldValue(fields[17]);
        if (fields.Length > 18) order.LaboratoryField2 = UnescapeFieldValue(fields[18]);
        
        if (fields.Length > 19 && !string.IsNullOrEmpty(fields[19]))
        {
            if (DateTime.TryParseExact(fields[19], "yyyyMMddHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var reportedDateTime))
            {
                order.ReportedDateTime = reportedDateTime;
            }
        }
        
        if (fields.Length > 20) order.InstrumentCharge = UnescapeFieldValue(fields[20]);
        if (fields.Length > 21) order.InstrumentSectionId = UnescapeFieldValue(fields[21]);
        if (fields.Length > 22) order.ReportTypes = UnescapeFieldValue(fields[22]);
        if (fields.Length > 23) order.Reserved = UnescapeFieldValue(fields[23]);
        if (fields.Length > 24) order.CollectionLocation = UnescapeFieldValue(fields[24]);
        if (fields.Length > 25) order.NosocomialInfectionFlag = UnescapeFieldValue(fields[25]);
        if (fields.Length > 26) order.SpecimenService = UnescapeFieldValue(fields[26]);
        if (fields.Length > 27) order.SpecimenInstitution = UnescapeFieldValue(fields[27]);
        
        return order;
    }
}