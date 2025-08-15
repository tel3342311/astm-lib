namespace AstmLib.Utilities;

/// <summary>
/// ASTM protocol control characters used for communication between clinical laboratory instruments and computer systems.
/// Based on ASTM E1394 and CLSI LIS01-A2/LIS02-A2 specifications.
/// </summary>
public static class ControlCharacters
{
    /// <summary>
    /// ENQ (Enquiry) - ASCII 5 - Used to establish communication
    /// </summary>
    public static readonly string ENQ = char.ConvertFromUtf32(5);
    
    /// <summary>
    /// ACK (Acknowledge) - ASCII 6 - Positive acknowledgment
    /// </summary>
    public static readonly string ACK = char.ConvertFromUtf32(6);
    
    /// <summary>
    /// NAK (Negative Acknowledge) - ASCII 21 - Negative acknowledgment
    /// </summary>
    public static readonly string NAK = char.ConvertFromUtf32(21);
    
    /// <summary>
    /// STX (Start of Text) - ASCII 2 - Beginning of message frame
    /// </summary>
    public static readonly string STX = char.ConvertFromUtf32(2);
    
    /// <summary>
    /// ETX (End of Text) - ASCII 3 - End of message frame
    /// </summary>
    public static readonly string ETX = char.ConvertFromUtf32(3);
    
    /// <summary>
    /// EOT (End of Transmission) - ASCII 4 - End of transmission
    /// </summary>
    public static readonly string EOT = char.ConvertFromUtf32(4);
    
    /// <summary>
    /// LF (Line Feed) - ASCII 10 - Line feed character
    /// </summary>
    public static readonly string LF = char.ConvertFromUtf32(10);
    
    /// <summary>
    /// CR (Carriage Return) - ASCII 13 - Carriage return character
    /// </summary>
    public static readonly string CR = char.ConvertFromUtf32(13);
    
    /// <summary>
    /// CRLF (Carriage Return + Line Feed) - Message terminator sequence
    /// </summary>
    public static readonly string CRLF = CR + LF;
    
    /// <summary>
    /// ETB (End of Transmission Block) - ASCII 23 - Intermediate block terminator
    /// </summary>
    public static readonly string ETB = char.ConvertFromUtf32(23);
    
    /// <summary>
    /// FS (Field Separator) - Pipe character | - Field separator in ASTM messages
    /// </summary>
    public static readonly string FS = "|";
    
    /// <summary>
    /// GS (Group Separator) - ASCII 29 - Component separator within fields
    /// </summary>
    public static readonly string GS = char.ConvertFromUtf32(29);
    
    /// <summary>
    /// RS (Record Separator) - ASCII 30 - Repeat separator for repeating fields
    /// </summary>
    public static readonly string RS = char.ConvertFromUtf32(30);
    
    /// <summary>
    /// US (Unit Separator) - ASCII 31 - Escape separator for special characters
    /// </summary>
    public static readonly string US = char.ConvertFromUtf32(31);
    
    /// <summary>
    /// Default timeout in milliseconds for ASTM communication
    /// </summary>
    public const int DefaultTimeoutMs = 15000;
    
    /// <summary>
    /// Maximum frame sequence number (0-7 for ASTM)
    /// </summary>
    public const int MaxFrameSequence = 7;
}