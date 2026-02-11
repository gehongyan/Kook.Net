using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Kook.Net.Queue;
using Kook.Net.Queue.InMemory;
using Xunit;

namespace Kook;

[Trait("Category", "Unit")]
public class BufferedInMemoryMessageQueueTests
{
    private static JsonElement CreatePayload(int value)
    {
        using JsonDocument doc = JsonDocument.Parse($"{{\"v\":{value}}}");
        return doc.RootElement.Clone();
    }

    /// <summary>
    /// 创建将 payload 中 "v" 入队到 order，并在 count 达到阈值时 signal 的 handler。
    /// </summary>
    private static Func<int, JsonElement, Task> CreateHandler(
        ConcurrentQueue<int> order,
        TaskCompletionSource? tcs = null,
        int signalWhenCount = 0)
    {
        return (_, payload) =>
        {
            if (payload.TryGetProperty("v", out JsonElement p) && p.TryGetInt32(out int v))
                order.Enqueue(v);
            if (signalWhenCount > 0 && order.Count >= signalWhenCount)
                tcs?.TrySetResult();
            return Task.CompletedTask;
        };
    }

    private static MessageQueueProvider CreateBufferedProvider(InMemoryMessageQueueOptions? options = null)
    {
        return InMemoryMessageQueueProvider.Create(options ?? new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 16,
        });
    }

    [Fact]
    public async Task Processes_messages_in_sequence_order()
    {
        ConcurrentQueue<int> order = [];
        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = CreateBufferedProvider();
        BaseMessageQueue queue = provider(CreateHandler(order, tcs, signalWhenCount: 3));

        await queue.StartAsync(TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(10), 1, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(20), 2, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(30), 3, TestContext.Current.CancellationToken);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        await queue.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal([10, 20, 30], order.ToArray());
    }

    [Fact]
    public async Task Reorders_buffered_messages_and_processes_in_sequence()
    {
        ConcurrentQueue<int> order = [];
        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = CreateBufferedProvider();
        BaseMessageQueue queue = provider(CreateHandler(order, tcs, signalWhenCount: 3));

        await queue.StartAsync(TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(10), 1, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(30), 3, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(20), 2, TestContext.Current.CancellationToken);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        await queue.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal([10, 20, 30], order.ToArray());
    }

    [Fact]
    public async Task Discards_duplicate_sequence_already_processed()
    {
        ConcurrentQueue<int> order = [];
        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = CreateBufferedProvider();
        BaseMessageQueue queue = provider(CreateHandler(order, tcs, signalWhenCount: 2));

        await queue.StartAsync(TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(10), 1, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(20), 2, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(99), 1, TestContext.Current.CancellationToken);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        await queue.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal([10, 20], order.ToArray());
    }

    [Fact]
    public async Task Create_with_null_options_returns_working_unbuffered_queue()
    {
        ConcurrentQueue<int> received = [];
        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider fromCreate = InMemoryMessageQueueProvider.Create();
        BaseMessageQueue queue = fromCreate(CreateHandler(received, tcs, signalWhenCount: 1));

        await queue.StartAsync(TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(42), 1, TestContext.Current.CancellationToken);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        await queue.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal([42], received.ToArray());
    }

    [Fact]
    public async Task BufferOverflowStrategy_DropIncoming_drops_new_message_when_buffer_full()
    {
        ConcurrentQueue<int> order = [];
        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 3,
            BufferOverflowStrategy = BufferOverflowStrategy.DropIncoming,
        });

        BaseMessageQueue queue = provider(CreateHandler(order, tcs, signalWhenCount: 5));

        await queue.StartAsync(TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(10), 1, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(30), 3, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(40), 4, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(50), 5, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(99), 6, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(20), 2, TestContext.Current.CancellationToken);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        await queue.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal([10, 20, 30, 40, 50], order.ToArray());
    }

    [Fact]
    public async Task BufferOverflowStrategy_ShiftOne_throws_when_buffer_full()
    {
        ConcurrentQueue<int> order = [];

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 2,
            BufferOverflowStrategy = BufferOverflowStrategy.ShiftOne,
        });

        BaseMessageQueue queue = provider(CreateHandler(order));

        await queue.StartAsync(TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(10), 1, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(30), 3, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(40), 4, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(50), 5, TestContext.Current.CancellationToken);
        await queue.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal([10, 30], order.ToArray());
    }

    [Fact]
    public async Task BufferOverflowStrategy_ThrowException_throws_when_buffer_full()
    {
        ConcurrentQueue<int> order = [];

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 2,
            BufferOverflowStrategy = BufferOverflowStrategy.ThrowException,
        });

        BaseMessageQueue queue = provider(CreateHandler(order));

        await queue.StartAsync(TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(10), 1, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(30), 3, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(40), 4, TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await queue.EnqueueAsync(CreatePayload(99), 5, TestContext.Current.CancellationToken));
        await queue.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal([10], order.ToArray());
    }

    [Fact]
    public async Task BufferWaitTimeoutStrategy_SkipMissingAndProcessBuffered_processes_buffered_on_timeout()
    {
        ConcurrentQueue<int> order = [];
        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 16,
            WaitForMissingTimeout = TimeSpan.FromMilliseconds(80),
            BufferWaitTimeoutStrategy = BufferWaitTimeoutStrategy.SkipMissing,
        });

        BaseMessageQueue queue = provider(CreateHandler(order, tcs, signalWhenCount: 2));

        await queue.StartAsync(TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(20), 2, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(40), 4, TestContext.Current.CancellationToken);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        await queue.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal([20, 40], order.ToArray());
    }

    [Fact]
    public async Task When_WaitForMissingTimeout_null_never_fires_timeout_keeps_waiting()
    {
        ConcurrentQueue<int> order = [];
        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 16,
            WaitForMissingTimeout = null,
        });

        BaseMessageQueue queue = provider(CreateHandler(order, tcs, signalWhenCount: 3));

        await queue.StartAsync(TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(20), 2, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(40), 4, TestContext.Current.CancellationToken);
        await Task.Delay(120, TestContext.Current.CancellationToken);
        Assert.Equal([20], order.ToArray());
        await queue.EnqueueAsync(CreatePayload(30), 3, TestContext.Current.CancellationToken);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        await queue.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal([20, 30, 40], order.ToArray());
    }

    [Fact]
    public async Task When_WaitForMissingTimeout_InfiniteTimeSpan_never_fires_timeout_keeps_waiting()
    {
        ConcurrentQueue<int> order = [];
        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 16,
            WaitForMissingTimeout = Timeout.InfiniteTimeSpan,
        });

        BaseMessageQueue queue = provider(CreateHandler(order, tcs, signalWhenCount: 3));

        await queue.StartAsync(TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(20), 2, TestContext.Current.CancellationToken);
        await queue.EnqueueAsync(CreatePayload(40), 4, TestContext.Current.CancellationToken);
        await Task.Delay(120, TestContext.Current.CancellationToken);
        Assert.Equal([20], order.ToArray());
        await queue.EnqueueAsync(CreatePayload(30), 3, TestContext.Current.CancellationToken);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        await queue.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal([20, 30, 40], order.ToArray());
    }
}
