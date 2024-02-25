using System.Net;
using System.Net.Sockets;

namespace Kook.Net.Udp;

internal class DefaultUdpSocket : IUdpSocket, IDisposable
{
    public event Func<byte[], int, int, Task> ReceivedDatagram;

    private readonly SemaphoreSlim _lock;
    private UdpClient _udp;
    private IPEndPoint _destination;
    private CancellationTokenSource _stopCancellationTokenSource, _cancellationTokenSource;
    private CancellationToken _cancellationToken, _parentToken;
    private Task _task;
    private bool _isDisposed;

    public ushort Port => (ushort)((_udp?.Client.LocalEndPoint as IPEndPoint)?.Port ?? 0);

    public DefaultUdpSocket()
    {
        _lock = new SemaphoreSlim(1, 1);
        _stopCancellationTokenSource = new CancellationTokenSource();
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                StopInternalAsync(true).GetAwaiter().GetResult();
                _stopCancellationTokenSource?.Dispose();
                _cancellationTokenSource?.Dispose();
                _lock?.Dispose();
            }

            _isDisposed = true;
        }
    }

    public void Dispose() => Dispose(true);


    public async Task StartAsync()
    {
        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            await StartInternalAsync(_cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task StartInternalAsync(CancellationToken cancellationToken)
    {
        await StopInternalAsync().ConfigureAwait(false);

        _stopCancellationTokenSource?.Dispose();
        _cancellationTokenSource?.Dispose();

        _stopCancellationTokenSource = new CancellationTokenSource();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _stopCancellationTokenSource.Token);
        _cancellationToken = _cancellationTokenSource.Token;

        _udp?.Dispose();
        _udp = new UdpClient(0);

        _task = RunAsync(_cancellationToken);
    }

    public async Task StopAsync()
    {
        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            await StopInternalAsync().ConfigureAwait(false);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task StopInternalAsync(bool isDisposing = false)
    {
        try
        {
            _stopCancellationTokenSource.Cancel(false);
        }
        catch
        {
            // ignored
        }

        if (!isDisposing) await (_task ?? Task.Delay(0)).ConfigureAwait(false);

        if (_udp != null)
        {
            try
            {
                _udp.Dispose();
            }
            catch
            {
                // ignored
            }

            _udp = null;
        }
    }

    public void SetDestination(string ip, int port) =>
        _destination = new IPEndPoint(IPAddress.Parse(ip), port);

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
        _cancellationTokenSource?.Dispose();

        _parentToken = cancellationToken;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _stopCancellationTokenSource.Token);
        _cancellationToken = _cancellationTokenSource.Token;
    }

    public async Task SendAsync(byte[] data, int index, int count)
    {
        if (index != 0) //Should never happen?
        {
            byte[] newData = new byte[count];
            Buffer.BlockCopy(data, index, newData, 0, count);
            data = newData;
        }

        await _udp.SendAsync(data, count, _destination).ConfigureAwait(false);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        Task closeTask = Task.Delay(-1, cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            Task<UdpReceiveResult> receiveTask = _udp.ReceiveAsync();

            _ = receiveTask.ContinueWith((receiveResult) =>
            {
                //observe the exception as to not receive as unhandled exception
                _ = receiveResult.Exception;
            }, TaskContinuationOptions.OnlyOnFaulted);

            Task task = await Task.WhenAny(closeTask, receiveTask).ConfigureAwait(false);
            if (task == closeTask) break;

            UdpReceiveResult result = receiveTask.Result;
            if (ReceivedDatagram != null)
                await ReceivedDatagram(result.Buffer, 0, result.Buffer.Length).ConfigureAwait(false);
        }
    }
}
