using System.Collections.Concurrent;
#if DEBUG_LIMITS
using System.Diagnostics;
#endif

#pragma warning disable CS8073

namespace Kook.Net.Queue;

internal class RequestQueue : IDisposable, IAsyncDisposable
{
    public event Func<BucketId, RateLimitInfo?, string?, Task>? RateLimitTriggered;

    private readonly ConcurrentDictionary<BucketId, object> _buckets;
    private readonly SemaphoreSlim _tokenLock;
    private readonly CancellationTokenSource _cancellationTokenSource; //Dispose token
    private CancellationTokenSource _clearToken;
    private CancellationToken _parentToken;
    private CancellationTokenSource? _requestCancellationTokenSource;
    private CancellationToken _requestCancellationToken; //Parent token + Clear token
    private DateTimeOffset _waitUntil;

    private readonly Task _cleanupTask;

    public RequestQueue()
    {
        _tokenLock = new SemaphoreSlim(1, 1);

        _clearToken = new CancellationTokenSource();
        _cancellationTokenSource = new CancellationTokenSource();
        _requestCancellationToken = CancellationToken.None;
        _parentToken = CancellationToken.None;

        _buckets = new ConcurrentDictionary<BucketId, object>();

        _cleanupTask = RunCleanup();
    }

    public async Task SetCancellationTokenAsync(CancellationToken cancellationToken)
    {
        await _tokenLock.WaitAsync().ConfigureAwait(false);
        try
        {
            _parentToken = cancellationToken;
            _requestCancellationTokenSource?.Dispose();
            _requestCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _clearToken.Token);
            _requestCancellationToken = _requestCancellationTokenSource.Token;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    public async Task ClearAsync()
    {
        await _tokenLock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
        try
        {
            _clearToken?.Cancel();
            _clearToken?.Dispose();
            _clearToken = new CancellationTokenSource();
            _requestCancellationTokenSource?.Dispose();
            _requestCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_clearToken.Token, _parentToken);
            _requestCancellationToken = _requestCancellationTokenSource.Token;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    public async Task<Stream> SendAsync(RestRequest request)
    {
        CancellationTokenSource? createdTokenSource = null;
        if (request.Options.CancellationToken.CanBeCanceled)
        {
            createdTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_requestCancellationToken, request.Options.CancellationToken);
            request.Options.CancellationToken = createdTokenSource.Token;
        }
        else
            request.Options.CancellationToken = _requestCancellationToken;

        RequestBucket bucket = GetOrCreateBucket(request.Options, request);
        Stream result = await bucket.SendAsync(request).ConfigureAwait(false);
        createdTokenSource?.Dispose();
        return result;
    }

    public async Task SendAsync(WebSocketRequest request)
    {
        CancellationTokenSource? createdTokenSource = null;
        if (request.Options.CancellationToken.CanBeCanceled)
        {
            createdTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_requestCancellationToken, request.Options.CancellationToken);
            request.Options.CancellationToken = createdTokenSource.Token;
        }
        else
            request.Options.CancellationToken = _requestCancellationToken;

        RequestBucket bucket = GetOrCreateBucket(request.Options, request);
        await bucket.SendAsync(request).ConfigureAwait(false);
        createdTokenSource?.Dispose();
    }

    internal async Task EnterGlobalAsync(int id, RestRequest request)
    {
        int millis = (int)Math.Ceiling((_waitUntil - DateTimeOffset.UtcNow).TotalMilliseconds);
        if (millis > 0)
        {
#if DEBUG_LIMITS
                Debug.WriteLine($"[{id}] Sleeping {millis} ms (Pre-emptive) [Global]");
#endif
            await Task.Delay(millis).ConfigureAwait(false);
        }
    }

    internal void PauseGlobal(RateLimitInfo info) =>
        _waitUntil = DateTimeOffset.UtcNow.Add(info.ResetAfter ?? TimeSpan.Zero).Add(info.Lag ?? TimeSpan.Zero);

    internal async Task EnterGlobalAsync(int id, WebSocketRequest request)
    {
        //If this is a global request (unbucketed), it'll be dealt in EnterAsync
        BucketId? bucketId = request.Options.BucketId;
        if (bucketId is null) return;
        GatewayBucket requestBucket = GatewayBucket.Get(bucketId);
        if (requestBucket.Type == GatewayBucketType.Unbucketed) return;

        //It's not a global request, so need to remove one from global (per-session)
        GatewayBucket globalBucketType = GatewayBucket.Get(GatewayBucketType.Unbucketed);
        RequestOptions options = RequestOptions.CreateOrClone(request.Options);
        options.BucketId = globalBucketType.Id;
        WebSocketRequest globalRequest = new(null, [], false, false, options);
        RequestBucket globalBucket = GetOrCreateBucket(options, globalRequest);
        await globalBucket.TriggerAsync(id, globalRequest);
    }

    private RequestBucket GetOrCreateBucket(RequestOptions options, IRequest request)
    {
        BucketId? bucketId = options.BucketId;
        if (bucketId is null)
            throw new InvalidOperationException("BucketId is not set.");
        object obj = _buckets.GetOrAdd(bucketId, x => new RequestBucket(this, request, x));
        if (obj is BucketId hashBucket)
        {
            options.BucketId = hashBucket;
            return (RequestBucket)_buckets.GetOrAdd(hashBucket, x => new RequestBucket(this, request, x));
        }

        return (RequestBucket)obj;
    }

    internal async Task RaiseRateLimitTriggered(BucketId bucketId, RateLimitInfo? info, string? endpoint)
    {
        if (RateLimitTriggered is null) return;
        await RateLimitTriggered.Invoke(bucketId, info, endpoint).ConfigureAwait(false);
    }

    internal (RequestBucket?, BucketId?) UpdateBucketHash(BucketId id, string kookHash)
    {
        if (!id.IsHashBucket)
        {
            BucketId bucket = BucketId.Create(kookHash, id);
            RequestBucket hashReqQueue = (RequestBucket)_buckets.GetOrAdd(bucket, _buckets[id]);
            _buckets.AddOrUpdate(id, bucket, (_, _) => bucket);
            return (hashReqQueue, bucket);
        }

        return (null, null);
    }

    public void ClearGatewayBuckets()
    {
        foreach (GatewayBucketType gwBucket in (GatewayBucketType[])Enum.GetValues(typeof(GatewayBucketType)))
            _buckets.TryRemove(GatewayBucket.Get(gwBucket).Id, out _);
    }

    private async Task RunCleanup()
    {
        try
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;
                foreach (RequestBucket bucket in _buckets.Where(x => x.Value is RequestBucket).Select(x => (RequestBucket)x.Value))
                {
                    if ((now - bucket.LastAttemptAt).TotalMinutes > 1.0)
                    {
                        if (bucket.Id.IsHashBucket)
                        {
                            foreach (BucketId redirectBucket in _buckets.Where(x => x.Value == bucket.Id).Select(x => (BucketId)x.Value))
                                _buckets.TryRemove(redirectBucket, out _); //remove redirections if hash bucket
                        }

                        _buckets.TryRemove(bucket.Id, out _);
                    }
                }

                await Task.Delay(60000, _cancellationTokenSource.Token).ConfigureAwait(false); //Runs each minute
            }
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (ObjectDisposedException)
        {
            // ignored
        }
    }

    public void Dispose()
    {
        if (!(_cancellationTokenSource is null))
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cleanupTask.GetAwaiter().GetResult();
        }

        _tokenLock?.Dispose();
        _clearToken?.Dispose();
        _requestCancellationTokenSource?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (!(_cancellationTokenSource is null))
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            await _cleanupTask.ConfigureAwait(false);
        }

        _tokenLock?.Dispose();
        _clearToken?.Dispose();
        _requestCancellationTokenSource?.Dispose();
    }
}
