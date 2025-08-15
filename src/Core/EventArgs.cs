namespace AstmLib.Core;

/// <summary>
/// Event arguments for data received events
/// </summary>
public class DataReceivedEventArgs : EventArgs
{
    /// <summary>
    /// The received data
    /// </summary>
    public byte[] Data { get; }
    
    /// <summary>
    /// Timestamp when the data was received
    /// </summary>
    public DateTime Timestamp { get; }
    
    /// <summary>
    /// The received data as string (UTF-8 decoded)
    /// </summary>
    public string DataString => System.Text.Encoding.UTF8.GetString(Data);
    
    public DataReceivedEventArgs(byte[] data)
    {
        Data = data;
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// Event arguments for connection state change events
/// </summary>
public class ConnectionStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Previous connection state
    /// </summary>
    public ConnectionState PreviousState { get; }
    
    /// <summary>
    /// Current connection state
    /// </summary>
    public ConnectionState CurrentState { get; }
    
    /// <summary>
    /// Timestamp of the state change
    /// </summary>
    public DateTime Timestamp { get; }
    
    public ConnectionStateChangedEventArgs(ConnectionState previousState, ConnectionState currentState)
    {
        PreviousState = previousState;
        CurrentState = currentState;
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// Event arguments for communication error events
/// </summary>
public class CommunicationErrorEventArgs : EventArgs
{
    /// <summary>
    /// The exception that occurred
    /// </summary>
    public Exception Exception { get; }
    
    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; }
    
    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; }
    
    /// <summary>
    /// Whether the error is recoverable
    /// </summary>
    public bool IsRecoverable { get; }
    
    public CommunicationErrorEventArgs(Exception exception, bool isRecoverable = false)
    {
        Exception = exception;
        Message = exception.Message;
        Timestamp = DateTime.UtcNow;
        IsRecoverable = isRecoverable;
    }
    
    public CommunicationErrorEventArgs(string message, bool isRecoverable = false)
    {
        Exception = new Exception(message);
        Message = message;
        Timestamp = DateTime.UtcNow;
        IsRecoverable = isRecoverable;
    }
}

/// <summary>
/// Connection state enumeration
/// </summary>
public enum ConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Disconnecting,
    Error
}