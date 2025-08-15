using AstmLib.Core;
using AstmLib.Messages;
using AstmLib.Protocols.Astm1394;
using AstmLib.Protocols.ClsiLis01;
using AstmLib.Protocols.ClsiLis02;
using System.Text;

namespace SimpleDeviceSimulator;

/// <summary>
/// Simple medical device simulator demonstrating ASTM protocol usage
/// Shows how to create and serialize the three message types you requested
/// </summary>
class Program
{
    private static readonly Astm1394Protocol _protocol = new();

    static async Task Main(string[] args)
    {
        Console.WriteLine("🏥 Simple ASTM Medical Device Simulator");
        Console.WriteLine("========================================");
        Console.WriteLine();

        while (true)
        {
            ShowMenu();
            var choice = Console.ReadLine();

            try
            {
                switch (choice?.ToLower())
                {
                    case "1":
                        await GenerateConnectionMessage();
                        break;
                    case "2":
                        await GenerateOrderStatusMessage();
                        break;
                    case "3":
                        await GenerateRecordMessage();
                        break;
                    case "4":
                        await ParseRawMessages();
                        break;
                    case "5":
                        await TestDifferentProtocols();
                        break;
                    case "q":
                    case "quit":
                        Console.WriteLine("👋 Goodbye!");
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("📋 Select an option:");
        Console.WriteLine("1. Generate Connection Message");
        Console.WriteLine("2. Generate Order Status Message");
        Console.WriteLine("3. Generate Record Message (Lab Results)");
        Console.WriteLine("4. Parse Raw ASTM Messages");
        Console.WriteLine("5. Test Different Protocol Versions");
        Console.WriteLine("Q. Quit");
        Console.WriteLine();
        Console.Write("Enter your choice: ");
    }

    static async Task GenerateConnectionMessage()
    {
        Console.WriteLine("🔌 Generating Connection Message...");
        Console.WriteLine();

        // Create connection message matching your example
        var messages = new List<IAstmMessage>
        {
            new HeaderRecord
            {
                SequenceNumber = 1,
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

        await DisplayMessages(messages, "Connection");
    }

    static async Task GenerateOrderStatusMessage()
    {
        Console.WriteLine("📋 Generating Order Status Message...");
        Console.WriteLine();

        var messages = new List<IAstmMessage>
        {
            new HeaderRecord
            {
                SequenceNumber = 1,
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
                RequestedDateTime = DateTime.ParseExact("20250731054920", "yyyyMMddHHmmss", null),
                SpecimenDescriptor = "Diagnosis-II Panel"
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

        await DisplayMessages(messages, "Order Status");
    }

    static async Task GenerateRecordMessage()
    {
        Console.WriteLine("🧪 Generating Lab Results Message...");
        Console.WriteLine();

        var messages = new List<IAstmMessage>
        {
            new HeaderRecord
            {
                SequenceNumber = 1,
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
                Race = "Canine",
                Language = "123456789012345678901234567890123456789012345678"
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
            ("QC1", "min.90"), ("QC2", "min.90"), ("QC3", "min.90"), ("QC4", "min.90"),
            ("QC5", "min.90"), ("QC6", "min.90"), ("QC7", "90^110"), ("L340nm", "90^110"),
            ("L405nm", "90^110"), ("L450nm", "90^110"), ("L510nm", "90^110"),
            ("L540nm", "90^110"), ("L600nm", "90^110"), ("L650nm", "90^110"),
            ("L940nm", "90^110"), ("System_QC", "min.90"), ("Chemistry_QC", "min.90"),
            ("QC", "OK")
        };

        var completedDateTime = DateTime.ParseExact("20231219154736", "yyyyMMddHHmmss", null);
        for (int i = 0; i < testResults.Length; i++)
        {
            var (testName, referenceRange) = testResults[i];
            messages.Add(new ResultRecord
            {
                SequenceNumber = i + 1,
                UniversalTestId = $"^^^{testName}",
                ReferenceRanges = referenceRange,
                ResultStatus = "N",
                TestCompletedDateTime = completedDateTime
            });
        }

        // Add final truncated result (matching your example)
        messages.Add(new ResultRecord
        {
            SequenceNumber = 19,
            UniversalTestId = "^^^Samp",
            ResultStatus = "N"
        });

        messages.Add(new TerminatorRecord
        {
            SequenceNumber = 1,
            TerminationCode = TerminationCode.Normal
        });

        await DisplayMessages(messages, "Lab Results");
    }

    static async Task DisplayMessages(List<IAstmMessage> messages, string messageType)
    {
        // Serialize to ASTM protocol
        var astmString = await _protocol.SerializeToStringAsync(messages);
        var astmBytes = await _protocol.SerializeAsync(messages);

        Console.WriteLine($"📤 {messageType} Message Generated ({astmBytes.Length} bytes):");
        Console.WriteLine();
        Console.WriteLine("ASTM Protocol Output:");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine(astmString);
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();

        // Validate the messages
        var validation = await _protocol.ValidateAsync(messages);
        Console.WriteLine($"✅ Validation: {(validation.IsValid ? "VALID" : "INVALID")}");
        
        if (!validation.IsValid)
        {
            Console.WriteLine("❌ Errors:");
            foreach (var error in validation.Errors)
            {
                Console.WriteLine($"   - {error.Message}");
            }
        }

        if (validation.Warnings.Any())
        {
            Console.WriteLine("⚠️ Warnings:");
            foreach (var warning in validation.Warnings)
            {
                Console.WriteLine($"   - {warning.Message}");
            }
        }

        // Display message breakdown
        Console.WriteLine();
        Console.WriteLine("📋 Message Breakdown:");
        foreach (var msg in messages)
        {
            Console.WriteLine($"   {msg.RecordType}|{msg.SequenceNumber}: {msg.GetType().Name}");
        }
    }

    static async Task ParseRawMessages()
    {
        Console.WriteLine("🧪 Testing Raw Message Parsing...");
        Console.WriteLine();

        // Your raw message examples (simplified for parsing)
        var rawMessages = new Dictionary<string, string>
        {
            ["Connection"] = "H|\\^&|||skyla VB1^AS01.AAES27|||||||P|1394-97|20250703124051" +
                           "\rC|1|I|L1400430039^Connect|G" +
                           "\rL|1|N",
            
            ["Order Status"] = "H|\\^&|||skyla VB1^DS20|||||||P|1394-97|20250731054920" +
                             "\rP|1||PatientId||PatientName||20250701|M||||||Allen|Canine^dogg||12^Kg||||||||||||||||" +
                             "\rO|1|000166||Diagnosis-II Panel||20250731054920|||||||||Diagnosis-II Panel||||||||||||||" +
                             "\rC|1|I|Queued|G" +
                             "\rL|1|N"
        };

        foreach (var (name, rawData) in rawMessages)
        {
            Console.WriteLine($"📄 Parsing {name} Message:");
            Console.WriteLine($"Raw: {rawData}");
            Console.WriteLine();

            try
            {
                // For this demo, we'll manually frame each line since raw messages need proper ASTM framing
                var lines = rawData.Split('\r');
                var framedData = new StringBuilder();
                
                for (int i = 0; i < lines.Length; i++)
                {
                    if (string.IsNullOrEmpty(lines[i])) continue;
                    
                    var frameNumber = (i % 8) + 1;
                    var frame = _protocol.CreateFrame(lines[i], frameNumber);
                    framedData.Append(frame);
                    if (i < lines.Length - 1) framedData.Append("\r\n");
                }

                var parsedMessages = await _protocol.ParseAsync(framedData.ToString());
                
                Console.WriteLine($"✅ Parsed {parsedMessages.Count()} records:");
                foreach (var msg in parsedMessages)
                {
                    Console.WriteLine($"   {msg.RecordType}|{msg.SequenceNumber}: {msg.GetType().Name}");
                }

                // Validate parsed messages
                var validation = await _protocol.ValidateAsync(parsedMessages);
                Console.WriteLine($"Validation: {(validation.IsValid ? "✅ VALID" : "❌ INVALID")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Parsing failed: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    static async Task TestDifferentProtocols()
    {
        Console.WriteLine("🔧 Testing Different Protocol Versions...");
        Console.WriteLine();

        var testMessages = new List<IAstmMessage>
        {
            new HeaderRecord
            {
                SenderId = "Test Device",
                ProcessingId = ProcessingType.Test,
                VersionNumber = "1394-97"
            },
            new TerminatorRecord
            {
                TerminationCode = TerminationCode.Normal
            }
        };

        var protocols = new Dictionary<string, IAstmProtocol>
        {
            ["ASTM E1394"] = new Astm1394Protocol(),
            ["CLSI LIS01"] = new ClsiLis01Protocol(),
            ["CLSI LIS02 (Strict)"] = new ClsiLis02Protocol(enforceStrictValidation: true),
            ["CLSI LIS02 (Lenient)"] = new ClsiLis02Protocol(enforceStrictValidation: false)
        };

        foreach (var (protocolName, protocol) in protocols)
        {
            Console.WriteLine($"📋 Testing {protocolName} Protocol:");
            
            try
            {
                var output = await protocol.SerializeToStringAsync(testMessages);
                var validation = await protocol.ValidateAsync(testMessages);
                
                Console.WriteLine($"   Serialized: {output.Length} characters");
                Console.WriteLine($"   Validation: {(validation.IsValid ? "✅ VALID" : "❌ INVALID")}");
                Console.WriteLine($"   Version: {protocol.ProtocolVersion}");
                
                if (!validation.IsValid)
                {
                    foreach (var error in validation.Errors.Take(3))
                    {
                        Console.WriteLine($"      Error: {error.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error: {ex.Message}");
            }
            
            Console.WriteLine();
        }
    }
}