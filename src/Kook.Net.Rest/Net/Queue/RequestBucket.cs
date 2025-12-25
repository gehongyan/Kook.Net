using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Kook.Net.Contexts;
using Kook.Net.Converters;
using Kook.Net.Rest;
using Kook.Rest;

namespace Kook.Net.Queue;

internal class RequestBucket
{
    private const int MinimumSleepTimeMs = 750;

    private readonly object _lock;
    private readonly RequestQueue _queue;
    private int _semaphore;
    private DateTimeOffset? _resetTick;
    private RequestBucket? _redirectBucket;
    private readonly JsonSerializerOptions _serializerOptions;

    public BucketId Id { get; private set; }
    public int WindowCount { get; private set; }
    public DateTimeOffset LastAttemptAt { get; private set; }

    public RequestBucket(RequestQueue queue, IRequest request, BucketId id)
    {
        _serializerOptions = new JsonSerializerOptions(KookRestJsonSerializerContext.Default.Options)
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        _queue = queue;
        Id = id;

        _lock = new object();

        if (request.Options.IsClientBucket && request.Options.BucketId != null)
            WindowCount = ClientBucket.Get(request.Options.BucketId).WindowCount;
        else if (request.Options.IsGatewayBucket && request.Options.BucketId != null)
            WindowCount = GatewayBucket.Get(request.Options.BucketId).WindowCount;
        else
            WindowCount = 1; //Only allow one request until we get a header back

        _semaphore = WindowCount;
        _resetTick = null;
        LastAttemptAt = DateTimeOffset.UtcNow;
    }

    private static int nextId = 0;

    public async Task<Stream> SendAsync(RestRequest request)
    {
        int id = Interlocked.Increment(ref nextId);
        KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Start");
        LastAttemptAt = DateTimeOffset.UtcNow;
        while (true)
        {
            await _queue.EnterGlobalAsync(id, request).ConfigureAwait(false);
            await EnterAsync(id, request).ConfigureAwait(false);
            if (_redirectBucket != null)
                return await _redirectBucket.SendAsync(request);

            KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Sending...");
            RestResponse response = default;
            RateLimitInfo info = default;
            try
            {
                response = await request.SendAsync().ConfigureAwait(false);
                info = new RateLimitInfo(response.Headers, request.Endpoint);

                request.Options.ExecuteRatelimitCallback(info);

                if (response.StatusCode < (HttpStatusCode)200 || response.StatusCode >= (HttpStatusCode)300)
                    switch (response.StatusCode)
                    {
                        case (HttpStatusCode)429:
                            if (info.IsGlobal)
                            {
                                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] (!) 429 [Global]");
                                _queue.PauseGlobal(info);
                            }
                            else
                            {
                                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] (!) 429");
                            }

                            await _queue.RaiseRateLimitTriggered(Id, info, $"{request.Method} {request.Endpoint}").ConfigureAwait(false);
                            continue;                   //Retry
                        case HttpStatusCode.BadGateway: //502
                            KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] (!) 502");
                            if ((request.Options.RetryMode & RetryMode.Retry502) == 0)
                                throw new HttpException(HttpStatusCode.BadGateway, request, null);

                            continue; //Retry
                        default:
                            API.Rest.RestResponseBase? responseBase = null;
                            if (response.Stream != null)
                                try
                                {
                                    responseBase = await JsonSerializer.DeserializeAsync(
                                        response.Stream, _serializerOptions.GetTypedTypeInfo<API.Rest.RestResponseBase>());
                                }
                                catch
                                {
                                    // ignored
                                }

                            throw new HttpException(
                                response.StatusCode,
                                request,
                                responseBase?.Code,
                                responseBase?.Message,
                                responseBase?.Data is not null
                                    ? [new KookJsonError("root", [new KookError(((int)responseBase.Code).ToString(), responseBase.Message)])]
                                    : null
                            );
                    }
                else
                {
                    KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Success");
                    if (response.MediaTypeHeader?.MediaType == "application/json")
                    {
                        API.Rest.RestResponseBase? responseBase = await JsonSerializer.DeserializeAsync(
                            response.Stream, _serializerOptions.GetTypedTypeInfo<API.Rest.RestResponseBase>());
                        if (responseBase?.Code > (KookErrorCode)0)
                        {
                            throw new HttpException(
                                response.StatusCode,
                                request,
                                responseBase.Code,
                                responseBase.Message,
                                [new KookJsonError("root", [new KookError(((int)responseBase.Code).ToString(), responseBase.Message)])]
                            );
                        }

                        return new MemoryStream(Encoding.UTF8.GetBytes(responseBase?.Data.ToString() ?? string.Empty));
                    }
                    else if (response.MediaTypeHeader?.MediaType == "image/svg+xml")
                        return response.Stream;
                }
            }
            //catch (HttpException) { throw; } //Pass through
            catch (TimeoutException)
            {
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Timeout");
                if ((request.Options.RetryMode & RetryMode.RetryTimeouts) == 0)
                    throw;

                await Task.Delay(500).ConfigureAwait(false);
                continue; //Retry
            }
            /*catch (Exception)
            {
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Error");
                if ((request.Options.RetryMode & RetryMode.RetryErrors) == 0)
                    throw;

                await Task.Delay(500);
                continue; //Retry
            }*/
            finally
            {
                UpdateRateLimit(id, request, info, response.StatusCode == (HttpStatusCode)429);
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Stop");
            }
        }
    }

    public async Task SendAsync(WebSocketRequest request)
    {
        int id = Interlocked.Increment(ref nextId);
        KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Start");
        LastAttemptAt = DateTimeOffset.UtcNow;
        while (true)
        {
            await _queue.EnterGlobalAsync(id, request).ConfigureAwait(false);
            await EnterAsync(id, request).ConfigureAwait(false);

            KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Sending...");
            try
            {
                await request.SendAsync().ConfigureAwait(false);
                return;
            }
            catch (TimeoutException)
            {
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Timeout");
                if ((request.Options.RetryMode & RetryMode.RetryTimeouts) == 0)
                    throw;

                await Task.Delay(500).ConfigureAwait(false);
                continue; //Retry
            }
            /*catch (Exception)
            {
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Error");
                if ((request.Options.RetryMode & RetryMode.RetryErrors) == 0)
                    throw;

                await Task.Delay(500);
                continue; //Retry
            }*/
            finally
            {
                UpdateRateLimit(id, request, default, false);
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Stop");
            }
        }
    }

    internal async Task TriggerAsync(int id, IRequest request)
    {
            KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Trigger Bucket");
        await EnterAsync(id, request).ConfigureAwait(false);
        UpdateRateLimit(id, request, default, false);
    }

    private async Task EnterAsync(int id, IRequest request)
    {
        int windowCount;
        DateTimeOffset? resetAt;
        bool isRateLimited = false;

        while (true)
        {
            if (_redirectBucket != null) break;

            if (DateTimeOffset.UtcNow > request.TimeoutAt || request.Options.CancellationToken.IsCancellationRequested)
            {
                if (!isRateLimited)
                    throw new TimeoutException();
                ThrowRetryLimit(request);
            }

            lock (_lock)
            {
                windowCount = WindowCount;
                resetAt = _resetTick;
            }

            DateTimeOffset? timeoutAt = request.TimeoutAt;
            int semaphore = Interlocked.Decrement(ref _semaphore);
            if (windowCount >= 0 && semaphore < 0)
            {
                if (!isRateLimited)
                {
                    bool ignoreRatelimit = false;
                    isRateLimited = true;
                    switch (request)
                    {
                        case RestRequest restRequest:
                            await _queue.RaiseRateLimitTriggered(Id, null, $"{restRequest.Method} {restRequest.Endpoint}").ConfigureAwait(false);
                            break;
                        case WebSocketRequest webSocketRequest:
                            if (webSocketRequest.IgnoreLimit)
                            {
                                ignoreRatelimit = true;
                                break;
                            }

                            await _queue.RaiseRateLimitTriggered(Id, null, Id.Endpoint).ConfigureAwait(false);
                            break;
                        default:
                            throw new InvalidOperationException("Unknown request type");
                    }

                    if (ignoreRatelimit)
                    {
                        KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Ignoring ratelimit");
                        break;
                    }
                }

                ThrowRetryLimit(request);

                if (resetAt.HasValue && resetAt > DateTimeOffset.UtcNow)
                {
                    if (resetAt > timeoutAt)
                        ThrowRetryLimit(request);

                    int millis = (int)Math.Ceiling((resetAt.Value - DateTimeOffset.UtcNow).TotalMilliseconds);
                    KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Sleeping {millis} ms (Pre-emptive)");
                    if (millis > 0) await Task.Delay(millis, request.Options.CancellationToken).ConfigureAwait(false);
                }
                else
                {
                    if ((timeoutAt - DateTimeOffset.UtcNow)?.TotalMilliseconds < MinimumSleepTimeMs)
                        ThrowRetryLimit(request);
                    KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Sleeping {MinimumSleepTimeMs}* ms (Pre-emptive)");
                    await Task.Delay(MinimumSleepTimeMs, request.Options.CancellationToken).ConfigureAwait(false);
                }

                continue;
            }

            KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Entered Semaphore ({semaphore}/{WindowCount} remaining)");
            break;
        }
    }

    private void UpdateRateLimit(int id, IRequest request, RateLimitInfo info, bool is429, bool redirected = false)
    {
        if (WindowCount == 0)
            return;

        lock (_lock)
        {
            KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Raw RateLimitInto: IsGlobal: {info.IsGlobal}, Limit: {info.Limit}, Remaining: {info.Remaining}, ResetAfter: {info.ResetAfter?.TotalSeconds}");
            if (redirected)
            {
                // we might still hit a real ratelimit if all tickets were already taken, can't do much about it since we didn't know they were the same
                Interlocked.Decrement(ref _semaphore);
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Decrease Semaphore");
            }

            bool hasQueuedReset = _resetTick != null;

            if (info.Bucket != null && !redirected)
            {
                (RequestBucket?, BucketId?) hashBucket = _queue.UpdateBucketHash(Id, info.Bucket);
                if (hashBucket.Item1 is not null && hashBucket.Item2 is not null)
                {
                    if (hashBucket.Item1 == this) //this bucket got promoted to a hash queue
                    {
                        Id = hashBucket.Item2;
                        KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Promoted to Hash Bucket ({hashBucket.Item2})");
                    }
                    else
                    {
                        // this request should be part of another bucket, this bucket will be disabled, redirect everything
                        _redirectBucket = hashBucket.Item1;
                        // update the hash bucket ratelimit
                        _redirectBucket.UpdateRateLimit(id, request, info, is429, true);
                        KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Redirected to {_redirectBucket.Id}");
                        return;
                    }
                }
            }

            if (info.Limit.HasValue && WindowCount != info.Limit.Value)
            {
                WindowCount = info.Limit.Value;
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Updated Limit to {WindowCount}");
            }

            if (info.Remaining.HasValue && _semaphore != info.Remaining.Value)
            {
                _semaphore = info.Remaining.Value;
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Updated Semaphore (Remaining) to {_semaphore}");
            }

            DateTimeOffset? resetTick = null;

            //Using X-Rate-Limit-Remaining causes a race condition
            /*if (info.Remaining.HasValue)
            {
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] X-Rate-Limit-Remaining: " + info.Remaining.Value);
                _semaphore = info.Remaining.Value;
            }*/
            if (is429)
            {
                // Stop all requests until the QueueReset task is complete
                _semaphore = 0;

                // Read the Reset-After header
                resetTick = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(info.ResetAfter?.TotalSeconds ?? 0));
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Reset-After: {info.ResetAfter} ({info.ResetAfter?.TotalMilliseconds} ms)");
            }
            //                 if (info.RetryAfter.HasValue)
            //                 {
            //                     //RetryAfter is more accurate than Reset, where available
            //                     resetTick = DateTimeOffset.UtcNow.AddSeconds(info.RetryAfter.Value);
            //                     KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Retry-After: {info.RetryAfter.Value} ({info.RetryAfter.Value} ms)");
            //                 }
            else if (info.ResetAfter.HasValue) // && (request.Options.UseSystemClock.HasValue && !request.Options.UseSystemClock.Value)
            {
                resetTick = DateTimeOffset.UtcNow.Add(info.ResetAfter.Value);
                    KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Reset-After: {info.ResetAfter.Value} ({info.ResetAfter?.TotalMilliseconds} ms)");
            }
            //                 else if (info.Reset.HasValue)
            //                 {
            //                     resetTick = info.Reset.Value.AddSeconds(info.Lag?.TotalSeconds ?? 1.0);
            //
            //                     /* millisecond precision makes this unnecessary, retaining in case of regression
            //                     if (request.Options.IsReactionBucket)
            //                         resetTick = DateTimeOffset.Now.AddMilliseconds(250);
            //                     */
            //
            //                     int diff = (int)(resetTick.Value - DateTimeOffset.UtcNow).TotalMilliseconds;
            //                     KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] X-Rate-Limit-Reset: {info.Reset.Value.ToUnixTimeSeconds()} ({diff} ms, {info.Lag?.TotalMilliseconds} ms lag)");
            //                 }
            else if (request.Options.IsClientBucket && Id != null)
            {
                resetTick = DateTimeOffset.UtcNow.AddSeconds(ClientBucket.Get(Id).WindowSeconds);
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Client Bucket ({ClientBucket.Get(Id).WindowSeconds * 1000} ms)");
            }
            else if (request.Options.IsGatewayBucket && request.Options.BucketId != null)
            {
                resetTick = DateTimeOffset.UtcNow.AddSeconds(GatewayBucket.Get(request.Options.BucketId).WindowSeconds);
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Gateway Bucket ({GatewayBucket.Get(request.Options.BucketId).WindowSeconds * 1000} ms)");
                if (!hasQueuedReset)
                {
                    _resetTick = resetTick;
                    LastAttemptAt = resetTick.Value;
                    KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Reset in {(int)Math.Ceiling((resetTick - DateTimeOffset.UtcNow).Value.TotalMilliseconds)} ms");
                    _ = QueueReset(id, (int)Math.Ceiling((_resetTick.Value - DateTimeOffset.UtcNow).TotalMilliseconds), request);
                }

                return;
            }

            if (resetTick == null)
            {
                WindowCount = -1; //No rate limit info, disable limits on this bucket
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Disabled Semaphore");
                return;
            }

            if (!hasQueuedReset || resetTick > _resetTick)
            {
                _resetTick = resetTick;
                LastAttemptAt = resetTick.Value; //Make sure we don't destroy this until after it's been reset
                KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] Reset in {(int)Math.Ceiling((resetTick - DateTimeOffset.UtcNow).Value.TotalMilliseconds)} ms");

                if (!hasQueuedReset)
                    _ = QueueReset(id, (int)Math.Ceiling((_resetTick.Value - DateTimeOffset.UtcNow).TotalMilliseconds), request);
            }
        }
    }

    private async Task QueueReset(int id, int millis, IRequest request)
    {
        if (_resetTick == null)
            return;

        while (true)
        {
            if (millis > 0)
                await Task.Delay(millis).ConfigureAwait(false);

            lock (_lock)
            {
                millis = (int)Math.Ceiling((_resetTick.Value - DateTimeOffset.UtcNow).TotalMilliseconds);
                if (millis <= 0) //Make sure we haven't gotten a more accurate reset time
                {
                    KookDebugger.DebugRatelimit($"[Ratelimit] [{id}] * Reset *");
                    _semaphore = WindowCount;
                    _resetTick = null;
                    return;
                }
            }
        }
    }

    private void ThrowRetryLimit(IRequest request)
    {
        if ((request.Options.RetryMode & RetryMode.RetryRatelimit) == 0)
            throw new RateLimitedException(request);
    }
}
