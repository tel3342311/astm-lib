namespace AstmLib.Core;

/// <summary>
/// Represents an ASTM protocol implementation for parsing and serializing messages
/// </summary>
public interface IAstmProtocol
{
    /// <summary>
    /// The protocol version (e.g., "ASTM E1394", "CLSI LIS01-A2", "CLSI LIS02-A2")
    /// </summary>
    string ProtocolVersion { get; }
    
    /// <summary>
    /// Parses raw ASTM data into structured message objects
    /// </summary>
    /// <param name="data">Raw ASTM data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of parsed ASTM messages</returns>
    Task<IEnumerable<IAstmMessage>> ParseAsync(byte[] data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Parses raw ASTM string data into structured message objects
    /// </summary>
    /// <param name="data">Raw ASTM string data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of parsed ASTM messages</returns>
    Task<IEnumerable<IAstmMessage>> ParseAsync(string data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Serializes ASTM messages into raw data format
    /// </summary>
    /// <param name="messages">Collection of ASTM messages to serialize</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Serialized ASTM data</returns>
    Task<byte[]> SerializeAsync(IEnumerable<IAstmMessage> messages, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Serializes ASTM messages into string format
    /// </summary>
    /// <param name="messages">Collection of ASTM messages to serialize</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Serialized ASTM string data</returns>
    Task<string> SerializeToStringAsync(IEnumerable<IAstmMessage> messages, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates a collection of ASTM messages according to protocol specifications
    /// </summary>
    /// <param name="messages">Messages to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<ValidationResult> ValidateAsync(IEnumerable<IAstmMessage> messages, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a message frame with proper framing characters and checksum
    /// </summary>
    /// <param name="content">Message content</param>
    /// <param name="frameNumber">Frame sequence number (0-7)</param>
    /// <returns>Properly framed message</returns>
    string CreateFrame(string content, int frameNumber);
    
    /// <summary>
    /// Extracts content from a framed message and validates checksum
    /// </summary>
    /// <param name="frame">Framed message</param>
    /// <returns>Extracted content and validation result</returns>
    (string content, bool isValid) ExtractFrame(string frame);
    
    /// <summary>
    /// Calculates checksum for message content
    /// </summary>
    /// <param name="content">Message content</param>
    /// <returns>Calculated checksum</returns>
    string CalculateChecksum(string content);
}