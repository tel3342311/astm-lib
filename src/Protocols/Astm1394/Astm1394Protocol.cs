using AstmLib.Core;
using AstmLib.Messages;
using AstmLib.Utilities;
using AstmLib.Validation;
using System.Text;

namespace AstmLib.Protocols.Astm1394;

/// <summary>
/// Implementation of ASTM E1394 protocol for clinical laboratory instrument communication
/// </summary>
public class Astm1394Protocol : IAstmProtocol
{
    private readonly ChecksumCalculator _checksumCalculator;
    
    /// <summary>
    /// The protocol version identifier
    /// </summary>
    public string ProtocolVersion => "ASTM E1394-97";
    
    /// <summary>
    /// Initializes the ASTM 1394 protocol with standard checksum calculation
    /// </summary>
    public Astm1394Protocol() : this(DeviceType.Standard)
    {
    }
    
    /// <summary>
    /// Initializes the ASTM 1394 protocol with device-specific checksum calculation
    /// </summary>
    /// <param name="deviceType">Type of device for checksum calculation</param>
    public Astm1394Protocol(DeviceType deviceType)
    {
        _checksumCalculator = ChecksumCalculator.CreateFor(deviceType);
    }
    
    /// <summary>
    /// Initializes the ASTM 1394 protocol with custom checksum calculator
    /// </summary>
    /// <param name="checksumCalculator">Custom checksum calculator</param>
    public Astm1394Protocol(ChecksumCalculator checksumCalculator)
    {
        _checksumCalculator = checksumCalculator ?? throw new ArgumentNullException(nameof(checksumCalculator));
    }
    
    /// <summary>
    /// Parses raw ASTM data into structured message objects
    /// </summary>
    public async Task<IEnumerable<IAstmMessage>> ParseAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var dataString = Encoding.UTF8.GetString(data);
        return await ParseAsync(dataString, cancellationToken);
    }
    
    /// <summary>
    /// Parses raw ASTM string data into structured message objects
    /// </summary>
    public async Task<IEnumerable<IAstmMessage>> ParseAsync(string data, CancellationToken cancellationToken = default)
    {
        var messages = new List<IAstmMessage>();
        
        if (string.IsNullOrEmpty(data))
            return messages;
        
        // Split data into individual frames
        var frames = ExtractFrames(data);
        
        foreach (var frame in frames)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Extract and validate frame content
            var (content, isValid) = ExtractFrame(frame);
            
            if (!isValid)
            {
                throw new InvalidOperationException($"Invalid frame checksum: {frame}");
            }
            
            // Parse the content into message
            var message = ParseMessage(content);
            if (message != null)
            {
                messages.Add(message);
            }
        }
        
        return messages;
    }
    
    /// <summary>
    /// Serializes ASTM messages into raw data format
    /// </summary>
    public async Task<byte[]> SerializeAsync(IEnumerable<IAstmMessage> messages, CancellationToken cancellationToken = default)
    {
        var result = await SerializeToStringAsync(messages, cancellationToken);
        return Encoding.UTF8.GetBytes(result);
    }
    
    /// <summary>
    /// Serializes ASTM messages into string format
    /// </summary>
    public async Task<string> SerializeToStringAsync(IEnumerable<IAstmMessage> messages, CancellationToken cancellationToken = default)
    {
        var result = new StringBuilder();
        var frameNumber = 1;
        
        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var messageString = message.ToAstmString();
            var frame = CreateFrame(messageString, frameNumber);
            
            result.Append(frame);
            result.Append(ControlCharacters.CRLF);
            
            frameNumber = (frameNumber % ControlCharacters.MaxFrameSequence) + 1;
        }
        
        return result.ToString();
    }
    
    /// <summary>
    /// Validates a collection of ASTM messages according to protocol specifications
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(IEnumerable<IAstmMessage> messages, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        var warnings = new List<string>();
        var messageList = messages.ToList();
        
        if (!messageList.Any())
        {
            errors.Add("Message collection cannot be empty");
            return ValidationResult.Failure(errors);
        }
        
        // Check for Header record at the beginning
        var firstMessage = messageList.First();
        if (firstMessage.RecordType != "H")
        {
            errors.Add("First message must be a Header record (H)");
        }
        
        // Check for Terminator record at the end
        var lastMessage = messageList.Last();
        if (lastMessage.RecordType != "L")
        {
            errors.Add("Last message must be a Terminator record (L)");
        }
        
        // Validate individual messages
        foreach (var message in messageList)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var messageValidation = message.ValidateDetailed();
            if (!messageValidation.IsValid)
            {
                errors.AddRange(messageValidation.Errors.Select(e => $"{message.RecordType}|{message.SequenceNumber}: {e.Message}"));
            }
            
            warnings.AddRange(messageValidation.Warnings.Select(w => $"{message.RecordType}|{message.SequenceNumber}: {w.Message}"));
        }
        
        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors.Select(e => new ValidationError(e)).ToList(),
            Warnings = warnings.Select(w => new ValidationWarning(w)).ToList()
        };
        
        return result;
    }
    
    /// <summary>
    /// Creates a message frame with proper framing characters and checksum
    /// </summary>
    public string CreateFrame(string content, int frameNumber)
    {
        if (frameNumber < 1 || frameNumber > ControlCharacters.MaxFrameSequence)
        {
            throw new ArgumentOutOfRangeException(nameof(frameNumber), 
                $"Frame number must be between 1 and {ControlCharacters.MaxFrameSequence}");
        }
        
        // ASTM frame format: STX + frame_number + content + ETX + checksum
        var frameContent = frameNumber.ToString() + content;
        var checksumContent = frameContent + ControlCharacters.ETX;
        var checksum = CalculateChecksum(checksumContent);
        
        return ControlCharacters.STX + frameContent + ControlCharacters.ETX + checksum;
    }
    
    /// <summary>
    /// Extracts content from a framed message and validates checksum
    /// </summary>
    public (string content, bool isValid) ExtractFrame(string frame)
    {
        if (string.IsNullOrEmpty(frame))
            return (string.Empty, false);
        
        // Frame should start with STX
        if (!frame.StartsWith(ControlCharacters.STX))
            return (string.Empty, false);
        
        // Frame should have minimum length: STX + frame_number + ETX + checksum (2 chars)
        if (frame.Length < 5)
            return (string.Empty, false);
        
        // Find ETX position
        var etxIndex = frame.LastIndexOf(ControlCharacters.ETX);
        if (etxIndex == -1 || etxIndex < 2)
            return (string.Empty, false);
        
        // Check if we have enough characters for checksum after ETX
        if (etxIndex + 3 > frame.Length)
            return (string.Empty, false);
        
        // Extract frame content (including frame number)
        var frameContent = frame.Substring(1, etxIndex - 1); // Skip STX, get content between STX and ETX
        var providedChecksum = frame.Substring(etxIndex + 1, 2); // 2-character checksum after ETX
        
        // Validate checksum
        var checksumContent = frameContent + ControlCharacters.ETX;
        var isValid = _checksumCalculator.Validate(checksumContent, providedChecksum);
        
        // Extract actual content (skip frame number which is first character)
        var content = frameContent.Length > 1 ? frameContent.Substring(1) : string.Empty;
        
        return (content, isValid);
    }
    
    /// <summary>
    /// Calculates checksum for message content
    /// </summary>
    public string CalculateChecksum(string content)
    {
        return _checksumCalculator.Calculate(content);
    }
    
    /// <summary>
    /// Extracts individual frames from raw data
    /// </summary>
    private List<string> ExtractFrames(string data)
    {
        var frames = new List<string>();
        var currentPos = 0;
        
        while (currentPos < data.Length)
        {
            // Find next STX
            var stxIndex = data.IndexOf(ControlCharacters.STX, currentPos);
            if (stxIndex == -1)
                break;
            
            // Find corresponding ETX
            var etxIndex = data.IndexOf(ControlCharacters.ETX, stxIndex + 1);
            if (etxIndex == -1)
                break;
            
            // Extract frame including checksum (2 characters after ETX)
            if (etxIndex + 3 <= data.Length)
            {
                var frameLength = etxIndex + 3 - stxIndex;
                var frame = data.Substring(stxIndex, frameLength);
                frames.Add(frame);
                currentPos = etxIndex + 3;
            }
            else
            {
                // Not enough characters for complete frame
                break;
            }
        }
        
        return frames;
    }
    
    /// <summary>
    /// Parses message content into appropriate message object
    /// </summary>
    private IAstmMessage? ParseMessage(string content)
    {
        if (string.IsNullOrEmpty(content))
            return null;
        
        var recordType = content.Substring(0, 1);
        
        return recordType switch
        {
            "H" => HeaderRecord.Parse(content),
            "P" => PatientRecord.Parse(content),
            "L" => TerminatorRecord.Parse(content),
            // TODO: Add other record types (O, R, C)
            _ => null
        };
    }
}