using AstmLib.Core;

namespace AstmLib.Messages;

/// <summary>
/// Patient Record (P) - ASTM E1394 Section 8.4.2
/// Contains patient demographic and identification information
/// </summary>
public class PatientRecord : AstmMessage
{
    /// <summary>
    /// Record type identifier - always "P" for Patient records
    /// </summary>
    public override string RecordType => "P";
    
    /// <summary>
    /// Practice assigned patient ID (Field 2) - unique identifier assigned by laboratory
    /// </summary>
    public string? PracticePatientId { get; set; }
    
    /// <summary>
    /// Laboratory assigned patient ID (Field 3) - unique identifier assigned by laboratory
    /// </summary>
    public string? LaboratoryPatientId { get; set; }
    
    /// <summary>
    /// Patient ID #3 (Field 4) - additional patient identifier
    /// </summary>
    public string? PatientId3 { get; set; }
    
    /// <summary>
    /// Patient name (Field 5) - patient's full name
    /// Format: Last^First^Middle^Suffix^Title
    /// </summary>
    public string? PatientName { get; set; }
    
    /// <summary>
    /// Mother's maiden name (Field 6) - patient's mother's maiden name
    /// </summary>
    public string? MothersMaidenName { get; set; }
    
    /// <summary>
    /// Date/Time of birth (Field 7) - patient's date and time of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// Patient sex (Field 8) - patient's gender
    /// </summary>
    public PatientSex? Sex { get; set; }
    
    /// <summary>
    /// Patient race (Field 9) - patient's racial/ethnic background
    /// </summary>
    public string? Race { get; set; }
    
    /// <summary>
    /// Patient address (Field 10) - patient's home address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Reserved field (Field 11) - reserved for future use
    /// </summary>
    public string? Reserved { get; set; }
    
    /// <summary>
    /// Patient phone number (Field 12) - patient's contact phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Attending physician ID (Field 13) - identifier of patient's physician
    /// </summary>
    public string? AttendingPhysicianId { get; set; }
    
    /// <summary>
    /// Special field 1 (Field 14) - laboratory-defined special field
    /// </summary>
    public string? SpecialField1 { get; set; }
    
    /// <summary>
    /// Special field 2 (Field 15) - laboratory-defined special field
    /// </summary>
    public string? SpecialField2 { get; set; }
    
    /// <summary>
    /// Patient height (Field 16) - patient's height
    /// </summary>
    public string? Height { get; set; }
    
    /// <summary>
    /// Patient weight (Field 17) - patient's weight
    /// </summary>
    public string? Weight { get; set; }
    
    /// <summary>
    /// Patient's known or suspected diagnosis (Field 18) - diagnosis information
    /// </summary>
    public string? Diagnosis { get; set; }
    
    /// <summary>
    /// Patient's active medications (Field 19) - current medication list
    /// </summary>
    public string? ActiveMedications { get; set; }
    
    /// <summary>
    /// Patient's diet (Field 20) - dietary restrictions or requirements
    /// </summary>
    public string? Diet { get; set; }
    
    /// <summary>
    /// Practice field 1 (Field 21) - practice-defined field
    /// </summary>
    public string? PracticeField1 { get; set; }
    
    /// <summary>
    /// Practice field 2 (Field 22) - practice-defined field
    /// </summary>
    public string? PracticeField2 { get; set; }
    
    /// <summary>
    /// Admission/discharge dates (Field 23) - hospital admission/discharge information
    /// </summary>
    public string? AdmissionDischargeDates { get; set; }
    
    /// <summary>
    /// Admission status (Field 24) - patient's admission status
    /// </summary>
    public string? AdmissionStatus { get; set; }
    
    /// <summary>
    /// Location (Field 25) - patient's location (room, ward, etc.)
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Alternative diagnostic code and classification (Field 26) - additional diagnosis codes
    /// </summary>
    public string? AlternativeDiagnosticCode { get; set; }
    
    /// <summary>
    /// Patient religion (Field 27) - patient's religious affiliation
    /// </summary>
    public string? Religion { get; set; }
    
    /// <summary>
    /// Marital status (Field 28) - patient's marital status
    /// </summary>
    public string? MaritalStatus { get; set; }
    
    /// <summary>
    /// Isolation status (Field 29) - patient's isolation requirements
    /// </summary>
    public string? IsolationStatus { get; set; }
    
    /// <summary>
    /// Language (Field 30) - patient's primary language
    /// </summary>
    public string? Language { get; set; }
    
    /// <summary>
    /// Validates Patient record specific fields
    /// </summary>
    protected override void ValidateSpecific(List<string> errors)
    {
        // At least one patient identifier should be provided
        if (string.IsNullOrEmpty(PracticePatientId) && 
            string.IsNullOrEmpty(LaboratoryPatientId) && 
            string.IsNullOrEmpty(PatientId3))
        {
            errors.Add("At least one patient identifier (Practice, Laboratory, or Patient ID #3) must be provided");
        }
        
        // Validate date of birth if provided
        if (DateOfBirth.HasValue && DateOfBirth.Value > DateTime.Now)
        {
            errors.Add("Date of birth cannot be in the future");
        }
    }
    
    /// <summary>
    /// Serializes the Patient record to ASTM format string
    /// </summary>
    public override string ToAstmString()
    {
        var dateOfBirthStr = DateOfBirth?.ToString("yyyyMMddHHmmss") ?? string.Empty;
        var sexStr = Sex switch
        {
            PatientSex.Male => "M",
            PatientSex.Female => "F",
            PatientSex.Unknown => "U",
            _ => string.Empty
        };
        
        return BuildFieldString(
            RecordType,                                // Field 1: Record Type
            EscapeFieldValue(PracticePatientId),       // Field 2: Practice Patient ID
            EscapeFieldValue(LaboratoryPatientId),     // Field 3: Laboratory Patient ID
            EscapeFieldValue(PatientId3),              // Field 4: Patient ID #3
            EscapeFieldValue(PatientName),             // Field 5: Patient Name
            EscapeFieldValue(MothersMaidenName),       // Field 6: Mother's Maiden Name
            dateOfBirthStr,                            // Field 7: Date/Time of Birth
            sexStr,                                    // Field 8: Patient Sex
            EscapeFieldValue(Race),                    // Field 9: Patient Race
            EscapeFieldValue(Address),                 // Field 10: Patient Address
            EscapeFieldValue(Reserved),                // Field 11: Reserved
            EscapeFieldValue(PhoneNumber),             // Field 12: Patient Phone
            EscapeFieldValue(AttendingPhysicianId),    // Field 13: Attending Physician ID
            EscapeFieldValue(SpecialField1),           // Field 14: Special Field 1
            EscapeFieldValue(SpecialField2),           // Field 15: Special Field 2
            EscapeFieldValue(Height),                  // Field 16: Patient Height
            EscapeFieldValue(Weight),                  // Field 17: Patient Weight
            EscapeFieldValue(Diagnosis),               // Field 18: Diagnosis
            EscapeFieldValue(ActiveMedications),       // Field 19: Active Medications
            EscapeFieldValue(Diet),                    // Field 20: Patient Diet
            EscapeFieldValue(PracticeField1),          // Field 21: Practice Field 1
            EscapeFieldValue(PracticeField2),          // Field 22: Practice Field 2
            EscapeFieldValue(AdmissionDischargeDates), // Field 23: Admission/Discharge Dates
            EscapeFieldValue(AdmissionStatus),         // Field 24: Admission Status
            EscapeFieldValue(Location),                // Field 25: Location
            EscapeFieldValue(AlternativeDiagnosticCode), // Field 26: Alternative Diagnostic Code
            EscapeFieldValue(Religion),                // Field 27: Religion
            EscapeFieldValue(MaritalStatus),           // Field 28: Marital Status
            EscapeFieldValue(IsolationStatus),         // Field 29: Isolation Status
            EscapeFieldValue(Language)                 // Field 30: Language
        );
    }
    
    /// <summary>
    /// Creates a deep copy of the Patient record
    /// </summary>
    public override IAstmMessage Clone()
    {
        return new PatientRecord
        {
            SequenceNumber = SequenceNumber,
            PracticePatientId = PracticePatientId,
            LaboratoryPatientId = LaboratoryPatientId,
            PatientId3 = PatientId3,
            PatientName = PatientName,
            MothersMaidenName = MothersMaidenName,
            DateOfBirth = DateOfBirth,
            Sex = Sex,
            Race = Race,
            Address = Address,
            Reserved = Reserved,
            PhoneNumber = PhoneNumber,
            AttendingPhysicianId = AttendingPhysicianId,
            SpecialField1 = SpecialField1,
            SpecialField2 = SpecialField2,
            Height = Height,
            Weight = Weight,
            Diagnosis = Diagnosis,
            ActiveMedications = ActiveMedications,
            Diet = Diet,
            PracticeField1 = PracticeField1,
            PracticeField2 = PracticeField2,
            AdmissionDischargeDates = AdmissionDischargeDates,
            AdmissionStatus = AdmissionStatus,
            Location = Location,
            AlternativeDiagnosticCode = AlternativeDiagnosticCode,
            Religion = Religion,
            MaritalStatus = MaritalStatus,
            IsolationStatus = IsolationStatus,
            Language = Language
        };
    }
    
    /// <summary>
    /// Parses an ASTM field string into a Patient record
    /// </summary>
    public static PatientRecord Parse(string fieldString)
    {
        var fields = ParseFieldString(fieldString);
        var patient = new PatientRecord();
        
        if (fields.Length > 1) patient.PracticePatientId = UnescapeFieldValue(fields[1]);
        if (fields.Length > 2) patient.LaboratoryPatientId = UnescapeFieldValue(fields[2]);
        if (fields.Length > 3) patient.PatientId3 = UnescapeFieldValue(fields[3]);
        if (fields.Length > 4) patient.PatientName = UnescapeFieldValue(fields[4]);
        if (fields.Length > 5) patient.MothersMaidenName = UnescapeFieldValue(fields[5]);
        
        if (fields.Length > 6 && !string.IsNullOrEmpty(fields[6]))
        {
            if (DateTime.TryParseExact(fields[6], "yyyyMMddHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var dob))
            {
                patient.DateOfBirth = dob;
            }
        }
        
        if (fields.Length > 7)
        {
            patient.Sex = fields[7] switch
            {
                "M" => PatientSex.Male,
                "F" => PatientSex.Female,
                "U" => PatientSex.Unknown,
                _ => null
            };
        }
        
        if (fields.Length > 8) patient.Race = UnescapeFieldValue(fields[8]);
        if (fields.Length > 9) patient.Address = UnescapeFieldValue(fields[9]);
        if (fields.Length > 10) patient.Reserved = UnescapeFieldValue(fields[10]);
        if (fields.Length > 11) patient.PhoneNumber = UnescapeFieldValue(fields[11]);
        if (fields.Length > 12) patient.AttendingPhysicianId = UnescapeFieldValue(fields[12]);
        if (fields.Length > 13) patient.SpecialField1 = UnescapeFieldValue(fields[13]);
        if (fields.Length > 14) patient.SpecialField2 = UnescapeFieldValue(fields[14]);
        if (fields.Length > 15) patient.Height = UnescapeFieldValue(fields[15]);
        if (fields.Length > 16) patient.Weight = UnescapeFieldValue(fields[16]);
        if (fields.Length > 17) patient.Diagnosis = UnescapeFieldValue(fields[17]);
        if (fields.Length > 18) patient.ActiveMedications = UnescapeFieldValue(fields[18]);
        if (fields.Length > 19) patient.Diet = UnescapeFieldValue(fields[19]);
        if (fields.Length > 20) patient.PracticeField1 = UnescapeFieldValue(fields[20]);
        if (fields.Length > 21) patient.PracticeField2 = UnescapeFieldValue(fields[21]);
        if (fields.Length > 22) patient.AdmissionDischargeDates = UnescapeFieldValue(fields[22]);
        if (fields.Length > 23) patient.AdmissionStatus = UnescapeFieldValue(fields[23]);
        if (fields.Length > 24) patient.Location = UnescapeFieldValue(fields[24]);
        if (fields.Length > 25) patient.AlternativeDiagnosticCode = UnescapeFieldValue(fields[25]);
        if (fields.Length > 26) patient.Religion = UnescapeFieldValue(fields[26]);
        if (fields.Length > 27) patient.MaritalStatus = UnescapeFieldValue(fields[27]);
        if (fields.Length > 28) patient.IsolationStatus = UnescapeFieldValue(fields[28]);
        if (fields.Length > 29) patient.Language = UnescapeFieldValue(fields[29]);
        
        return patient;
    }
}

/// <summary>
/// Patient sex enumeration
/// </summary>
public enum PatientSex
{
    /// <summary>
    /// Male patient
    /// </summary>
    Male,
    
    /// <summary>
    /// Female patient
    /// </summary>
    Female,
    
    /// <summary>
    /// Unknown or not specified
    /// </summary>
    Unknown
}