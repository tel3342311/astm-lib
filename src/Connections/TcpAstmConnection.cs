using AstmLib.Core;
using AstmLib.Utilities;
using System.Net.Sockets;
using System.Text;

namespace AstmLib.Connections;

/// <summary>
/// TCP/IP connection implementation for ASTM protocol communication
/// </summary>
public class TcpAstmConnection : IAstmConnection
{
    private readonly string _host;
    private readonly int _port;
    private TcpClient? _tcpClient;
    private NetworkStream? _networkStream;
    private volatile bool _isDisposed;
    private ConnectionState _connectionState = ConnectionState.Disconnected;
    private CancellationTokenSource? _readCancellationTokenSource;
    
    /// <summary>
    /// Indicates whether the connection is currently open
    /// </summary>
    public bool IsConnected => _tcpClient?.Connected == true && _networkStream != null;
    
    /// <summary>
    /// Connection timeout in milliseconds
    /// </summary>
    public int TimeoutMs { get; set; } = ControlCharacters.DefaultTimeoutMs;
    
    /// <summary>
    /// Event raised when data is received from the instrument
    /// </summary>
    public event EventHandler<DataReceivedEventArgs>? DataReceived;
    
    /// <summary>
    /// Event raised when the connection state changes
    /// </summary>
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;
    
    /// <summary>
    /// Event raised when an error occurs during communication
    /// </summary>
    public event EventHandler<CommunicationErrorEventArgs>? CommunicationError;
    
    /// <summary>
    /// Initializes a new TCP ASTM connection
    /// </summary>
    /// <param name="host">Host address or IP</param>
    /// <param name="port">Port number</param>
    public TcpAstmConnection(string host, int port)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _port = port;
        
        if (port <= 0 || port > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 1 and 65535");
        }
    }
    
    /// <summary>
    /// Establishes connection to the instrument
    /// </summary>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(TcpAstmConnection));
            
        if (IsConnected)
            return;
        
        try
        {
            SetConnectionState(ConnectionState.Connecting);
            
            _tcpClient = new TcpClient();
            _tcpClient.ReceiveTimeout = TimeoutMs;
            _tcpClient.SendTimeout = TimeoutMs;
            
            using var timeoutCts = new CancellationTokenSource(TimeoutMs);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            
            await _tcpClient.ConnectAsync(_host, _port, combinedCts.Token);
            _networkStream = _tcpClient.GetStream();
            
            SetConnectionState(ConnectionState.Connected);
            
            // Start background reading
            _readCancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => BackgroundReadAsync(_readCancellationTokenSource.Token), _readCancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            SetConnectionState(ConnectionState.Error);
            OnCommunicationError(new CommunicationErrorEventArgs(ex, false));
            throw;
        }
    }
    
    /// <summary>
    /// Closes the connection to the instrument
    /// </summary>
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_isDisposed || !IsConnected)
            return;
        
        try
        {
            SetConnectionState(ConnectionState.Disconnecting);
            
            // Stop background reading
            _readCancellationTokenSource?.Cancel();
            
            // Close network stream and TCP client
            _networkStream?.Close();
            _tcpClient?.Close();
            
            SetConnectionState(ConnectionState.Disconnected);
        }
        catch (Exception ex)
        {
            SetConnectionState(ConnectionState.Error);
            OnCommunicationError(new CommunicationErrorEventArgs(ex, true));
        }
        finally
        {
            _networkStream?.Dispose();
            _tcpClient?.Dispose();
            _readCancellationTokenSource?.Dispose();
            
            _networkStream = null;
            _tcpClient = null;
            _readCancellationTokenSource = null;
        }
    }
    
    /// <summary>
    /// Sends data to the instrument
    /// </summary>
    public async Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(TcpAstmConnection));
            
        if (!IsConnected)
            throw new InvalidOperationException("Connection is not established");
        
        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeoutMs);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            
            await _networkStream!.WriteAsync(data, combinedCts.Token);
            await _networkStream.FlushAsync(combinedCts.Token);
        }
        catch (Exception ex)
        {
            OnCommunicationError(new CommunicationErrorEventArgs(ex, true));
            throw;
        }
    }
    
    /// <summary>
    /// Sends string data to the instrument
    /// </summary>
    public async Task SendAsync(string data, CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        await SendAsync(bytes, cancellationToken);
    }
    
    /// <summary>
    /// Receives data from the instrument
    /// </summary>
    public async Task<byte[]> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(TcpAstmConnection));
            
        if (!IsConnected)
            throw new InvalidOperationException("Connection is not established");
        
        try
        {
            var buffer = new byte[4096];
            using var timeoutCts = new CancellationTokenSource(TimeoutMs);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            
            var bytesRead = await _networkStream!.ReadAsync(buffer, combinedCts.Token);
            
            if (bytesRead == 0)
                return Array.Empty<byte>();
            
            var result = new byte[bytesRead];
            Array.Copy(buffer, result, bytesRead);
            return result;
        }
        catch (Exception ex)
        {
            OnCommunicationError(new CommunicationErrorEventArgs(ex, true));
            throw;
        }
    }
    
    /// <summary>
    /// Receives string data from the instrument
    /// </summary>
    public async Task<string> ReceiveStringAsync(CancellationToken cancellationToken = default)
    {
        var bytes = await ReceiveAsync(cancellationToken);
        return Encoding.UTF8.GetString(bytes);
    }
    
    /// <summary>
    /// Background task for continuously reading data
    /// </summary>
    private async Task BackgroundReadAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];
        
        try
        {
            while (!cancellationToken.IsCancellationRequested && IsConnected)
            {
                var bytesRead = await _networkStream!.ReadAsync(buffer, cancellationToken);
                
                if (bytesRead == 0)
                {
                    // Connection closed by remote host
                    SetConnectionState(ConnectionState.Disconnected);
                    break;
                }
                
                var data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead);
                
                OnDataReceived(new DataReceivedEventArgs(data));
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            OnCommunicationError(new CommunicationErrorEventArgs(ex, true));
            SetConnectionState(ConnectionState.Error);
        }
    }
    
    /// <summary>
    /// Sets the connection state and raises the state changed event
    /// </summary>
    private void SetConnectionState(ConnectionState newState)
    {
        var previousState = _connectionState;
        _connectionState = newState;
        
        if (previousState != newState)
        {
            OnConnectionStateChanged(new ConnectionStateChangedEventArgs(previousState, newState));
        }
    }
    
    /// <summary>
    /// Raises the DataReceived event
    /// </summary>
    protected virtual void OnDataReceived(DataReceivedEventArgs e)
    {
        DataReceived?.Invoke(this, e);
    }
    
    /// <summary>
    /// Raises the ConnectionStateChanged event
    /// </summary>
    protected virtual void OnConnectionStateChanged(ConnectionStateChangedEventArgs e)
    {
        ConnectionStateChanged?.Invoke(this, e);
    }
    
    /// <summary>
    /// Raises the CommunicationError event
    /// </summary>
    protected virtual void OnCommunicationError(CommunicationErrorEventArgs e)
    {
        CommunicationError?.Invoke(this, e);
    }
    
    /// <summary>
    /// Disposes the connection resources
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;
        
        _isDisposed = true;
        
        try
        {
            DisconnectAsync().Wait(1000); // Wait up to 1 second for graceful disconnect
        }
        catch
        {
            // Ignore exceptions during disposal
        }
        
        _readCancellationTokenSource?.Dispose();
        _networkStream?.Dispose();
        _tcpClient?.Dispose();
    }
    
    /// <summary>
    /// Returns string representation of the connection
    /// </summary>
    public override string ToString()
    {
        return $"TcpAstmConnection({_host}:{_port}, State: {_connectionState})";
    }
}