# Simple ASTM Medical Device Simulator

This application demonstrates how to use the ASTM library to create and process the three types of medical device messages you requested.

## Your Message Examples

### 1. Connection Message
```
String message = "H|\\^&|||skyla VB1^AS01.AAES27|||||||P|1394-97|20250703124051\r" +
    "C|1|I|L1400430039^Connect|G\r" +
    "L|1|N";
```

### 2. Order Status Message  
```
H|\^&|||skyla VB1^DS20|||||||P|1394-97|20250731054920
P|1||PatientId||PatientName||20250701|M||||||Allen|Canine^dogg||12^Kg|||||||||||||||||
O|1|000166||Diagnosis-II Panel||20250731054920|||||||||Diagnosis-II Panel|||||||||||||||
C|1|I|Queued|G
L|1|N
```

### 3. Lab Results Message
```
H|\^&|||skyla Solution^DST18T|||||||P|1394-97|20231219154736
P|1||402||||^||||||||Canine||||||||||||||||||123456789012345678901234567890123456789012345678
O|1|||^^^Integrated Report|||||||N||||Patient||||||||^||F
R|1|^^^QC1|||min.90|N||||||20231219154736
R|2|^^^QC2|||min.90|N||||||20231219154736
...many more result records...
```

## How to Create These Messages Using the Library

### Basic Usage Example

```csharp
using AstmLib.Core;
using AstmLib.Messages;
using AstmLib.Protocols.Astm1394;

// Create protocol instance
var protocol = new Astm1394Protocol();

// 1. Connection Message
var connectionMessages = new List<IAstmMessage>
{
    new HeaderRecord
    {
        SenderId = "skyla VB1",
        SenderAddress = "AS01.AAES27", 
        ProcessingId = ProcessingType.Production,
        VersionNumber = "1394-97",
        Timestamp = DateTime.ParseExact("20250703124051", "yyyyMMddHHmmss", null)
    },
    new CommentRecord
    {
        SequenceNumber = 1,
        CommentSource = "I",
        CommentText = "L1400430039^Connect",
        CommentType = "G"
    },
    new TerminatorRecord
    {
        SequenceNumber = 1,
        TerminationCode = TerminationCode.Normal
    }
};

// Serialize to ASTM format
var astmData = await protocol.SerializeToStringAsync(connectionMessages);
Console.WriteLine(astmData);

// 2. Order Status Message  
var orderMessages = new List<IAstmMessage>
{
    new HeaderRecord
    {
        SenderId = "skyla VB1",
        SenderAddress = "DS20",
        ProcessingId = ProcessingType.Production,
        VersionNumber = "1394-97",
        Timestamp = DateTime.ParseExact("20250731054920", "yyyyMMddHHmmss", null)
    },
    new PatientRecord
    {
        SequenceNumber = 1,
        LaboratoryPatientId = "PatientId",
        PatientName = "PatientName",
        DateOfBirth = DateTime.ParseExact("20250701", "yyyyMMdd", null),
        Sex = PatientSex.Male,
        AttendingPhysicianId = "Allen",
        SpecialField1 = "Canine^dogg",
        Weight = "12^Kg"
    },
    new OrderRecord
    {
        SequenceNumber = 1,
        SpecimenId = "000166",
        UniversalTestId = "Diagnosis-II Panel",
        RequestedDateTime = DateTime.ParseExact("20250731054920", "yyyyMMddHHmmss", null)
    },
    new CommentRecord
    {
        SequenceNumber = 1,
        CommentSource = "I",
        CommentText = "Queued",
        CommentType = "G"
    },
    new TerminatorRecord
    {
        SequenceNumber = 1,
        TerminationCode = TerminationCode.Normal
    }
};

var orderAstmData = await protocol.SerializeToStringAsync(orderMessages);

// 3. Lab Results with Multiple Results
var labMessages = new List<IAstmMessage>
{
    new HeaderRecord
    {
        SenderId = "skyla Solution",
        SenderAddress = "DST18T",
        ProcessingId = ProcessingType.Production,
        VersionNumber = "1394-97",
        Timestamp = DateTime.ParseExact("20231219154736", "yyyyMMddHHmmss", null)
    },
    new PatientRecord
    {
        SequenceNumber = 1,
        LaboratoryPatientId = "402",
        Race = "Canine"
    },
    new OrderRecord
    {
        SequenceNumber = 1,
        UniversalTestId = "^^^Integrated Report",
        ActionCode = "N",
        OrderingPhysician = "Patient",
        ReportTypes = "F"
    }
};

// Add multiple result records
var testResults = new[]
{
    ("QC1", "min.90"), ("QC2", "min.90"), ("QC3", "min.90"), 
    ("L340nm", "90^110"), ("L405nm", "90^110"), ("System_QC", "min.90")
};

for (int i = 0; i < testResults.Length; i++)
{
    var (testName, range) = testResults[i];
    labMessages.Add(new ResultRecord
    {
        SequenceNumber = i + 1,
        UniversalTestId = $"^^^{testName}",
        ReferenceRanges = range,
        ResultStatus = "N",
        TestCompletedDateTime = DateTime.ParseExact("20231219154736", "yyyyMMddHHmmss", null)
    });
}

labMessages.Add(new TerminatorRecord
{
    SequenceNumber = 1,
    TerminationCode = TerminationCode.Normal
});

var labAstmData = await protocol.SerializeToStringAsync(labMessages);
```

## Key ASTM Library Features

### Available Message Types
- **HeaderRecord (H)** - Device identification and session info
- **PatientRecord (P)** - Patient demographics and IDs  
- **OrderRecord (O)** - Test orders and specimen information
- **ResultRecord (R)** - Laboratory test results
- **CommentRecord (C)** - Additional comments and notes
- **TerminatorRecord (L)** - End of transmission marker

### Protocol Support
- **ASTM E1394-97** - Standard ASTM protocol
- **CLSI LIS01-A2** - Enhanced with validation
- **CLSI LIS02-A2** - Strict validation with session control

### Message Validation
```csharp
var validation = await protocol.ValidateAsync(messages);
if (!validation.IsValid)
{
    foreach (var error in validation.Errors)
    {
        Console.WriteLine($"Error: {error.Message}");
    }
}
```

### Parse Existing ASTM Data
```csharp
// Parse framed ASTM data back to message objects
var parsedMessages = await protocol.ParseAsync(astmData);
foreach (var message in parsedMessages)
{
    Console.WriteLine($"Parsed: {message.RecordType} - {message.GetType().Name}");
}
```

## Integration with Your Device

To use this library in your medical device application:

1. **Add the Library**: Reference `AstmLib.dll` in your project
2. **Create Messages**: Use the message classes to build your data
3. **Serialize**: Convert to ASTM protocol format for transmission
4. **Send**: Transmit over TCP, serial, or your preferred communication method
5. **Parse**: Parse incoming ASTM data back to structured objects
6. **Validate**: Ensure messages meet protocol requirements

The library handles all ASTM framing, checksums, field encoding, and protocol validation automatically.

## Running the Demo

Due to build environment issues, the console application may not run directly, but you can use the code examples above in your own C# application to generate the exact message formats you need.