namespace AstmLib.Core;

/// <summary>
/// Represents a connection to an ASTM-compliant clinical laboratory instrument
/// </summary>
public interface IAstmConnection : IDisposable
{
    /// <summary>
    /// Indicates whether the connection is currently open
    /// </summary>
    bool IsConnected { get; }
    
    /// <summary>
    /// Connection timeout in milliseconds
    /// </summary>
    int TimeoutMs { get; set; }
    
    /// <summary>
    /// Event raised when data is received from the instrument
    /// </summary>
    event EventHandler<DataReceivedEventArgs>? DataReceived;
    
    /// <summary>
    /// Event raised when the connection state changes
    /// </summary>
    event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;
    
    /// <summary>
    /// Event raised when an error occurs during communication
    /// </summary>
    event EventHandler<CommunicationErrorEventArgs>? CommunicationError;
    
    /// <summary>
    /// Establishes connection to the instrument
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the connection operation</returns>
    Task ConnectAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Closes the connection to the instrument
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the disconnection operation</returns>
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sends data to the instrument
    /// </summary>
    /// <param name="data">Data to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the send operation</returns>
    Task SendAsync(byte[] data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sends data to the instrument
    /// </summary>
    /// <param name="data">String data to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the send operation</returns>
    Task SendAsync(string data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Receives data from the instrument
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Received data</returns>
    Task<byte[]> ReceiveAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Receives data from the instrument as string
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Received data as string</returns>
    Task<string> ReceiveStringAsync(CancellationToken cancellationToken = default);
}