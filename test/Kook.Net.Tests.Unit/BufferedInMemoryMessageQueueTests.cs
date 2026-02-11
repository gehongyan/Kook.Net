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

    [Fact]
    public async Task Processes_messages_in_sequence_order()
    {
        var order = new ConcurrentQueue<int>();
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 16,
        });

        BaseMessageQueue queue = provider((_, payload) =>
        {
            if (payload.TryGetProperty("v", out JsonElement p) && p.TryGetInt32(out int v))
                order.Enqueue(v);
            if (order.Count >= 3)
                tcs.TrySetResult();
            return Task.CompletedTask;
        });

        await queue.StartAsync();
        await queue.EnqueueAsync(CreatePayload(10), 1);
        await queue.EnqueueAsync(CreatePayload(20), 2);
        await queue.EnqueueAsync(CreatePayload(30), 3);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await queue.StopAsync();

        Assert.Equal([10, 20, 30], order.ToArray());
    }

    [Fact]
    public async Task Reorders_buffered_messages_and_processes_in_sequence()
    {
        var order = new ConcurrentQueue<int>();
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 16,
        });

        BaseMessageQueue queue = provider((_, payload) =>
        {
            if (payload.TryGetProperty("v", out JsonElement p) && p.TryGetInt32(out int v))
                order.Enqueue(v);
            if (order.Count >= 3)
                tcs.TrySetResult();
            return Task.CompletedTask;
        });

        await queue.StartAsync();
        await queue.EnqueueAsync(CreatePayload(10), 1);
        await queue.EnqueueAsync(CreatePayload(30), 3);
        await queue.EnqueueAsync(CreatePayload(20), 2);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await queue.StopAsync();

        Assert.Equal([10, 20, 30], order.ToArray());
    }

    [Fact]
    public async Task Discards_duplicate_sequence_already_processed()
    {
        var order = new ConcurrentQueue<int>();
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 16,
        });

        BaseMessageQueue queue = provider((_, payload) =>
        {
            if (payload.TryGetProperty("v", out JsonElement p) && p.TryGetInt32(out int v))
                order.Enqueue(v);
            if (order.Count >= 2)
                tcs.TrySetResult();
            return Task.CompletedTask;
        });

        await queue.StartAsync();
        await queue.EnqueueAsync(CreatePayload(10), 1);
        await queue.EnqueueAsync(CreatePayload(20), 2);
        await queue.EnqueueAsync(CreatePayload(99), 1);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await queue.StopAsync();

        Assert.Equal([10, 20], order.ToArray());
    }

    [Fact]
    public async Task Create_with_null_options_returns_working_unbuffered_queue()
    {
        var received = new ConcurrentQueue<int>();
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider fromCreate = InMemoryMessageQueueProvider.Create(null);
        BaseMessageQueue queue = fromCreate((_, payload) =>
        {
            if (payload.TryGetProperty("v", out JsonElement p) && p.TryGetInt32(out int v))
                received.Enqueue(v);
            tcs.TrySetResult();
            return Task.CompletedTask;
        });

        await queue.StartAsync();
        await queue.EnqueueAsync(CreatePayload(42), 1);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await queue.StopAsync();

        Assert.Equal([42], received.ToArray());
    }

    [Fact]
    public async Task BufferOverflowStrategy_DropIncoming_drops_new_message_when_buffer_full()
    {
        var order = new ConcurrentQueue<int>();
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 3,
            BufferOverflowStrategy = BufferOverflowStrategy.DropIncoming,
        });

        BaseMessageQueue queue = provider((_, payload) =>
        {
            if (payload.TryGetProperty("v", out JsonElement p) && p.TryGetInt32(out int v))
                order.Enqueue(v);
            if (order.Count >= 5)
                tcs.TrySetResult();
            return Task.CompletedTask;
        });

        await queue.StartAsync();
        await queue.EnqueueAsync(CreatePayload(10), 1);
        await queue.EnqueueAsync(CreatePayload(30), 3);
        await queue.EnqueueAsync(CreatePayload(40), 4);
        await queue.EnqueueAsync(CreatePayload(50), 5);
        await queue.EnqueueAsync(CreatePayload(99), 6);
        await queue.EnqueueAsync(CreatePayload(20), 2);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await queue.StopAsync();

        Assert.Equal([10, 20, 30, 40, 50], order.ToArray());
    }

    [Fact]
    public async Task BufferOverflowStrategy_Throw_throws_when_buffer_full()
    {
        var order = new ConcurrentQueue<int>();

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 2,
            BufferOverflowStrategy = BufferOverflowStrategy.ThrowException,
        });

        BaseMessageQueue queue = provider((_, payload) =>
        {
            if (payload.TryGetProperty("v", out JsonElement p) && p.TryGetInt32(out int v))
                order.Enqueue(v);
            return Task.CompletedTask;
        });

        await queue.StartAsync();
        await queue.EnqueueAsync(CreatePayload(10), 1);
        await queue.EnqueueAsync(CreatePayload(30), 3);
        await queue.EnqueueAsync(CreatePayload(40), 4);
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await queue.EnqueueAsync(CreatePayload(99), 5));
        await queue.StopAsync();

        Assert.Equal([10], order.ToArray());
    }

    [Fact]
    public async Task BufferWaitTimeoutStrategy_SkipMissingAndProcessBuffered_processes_buffered_on_timeout()
    {
        var order = new ConcurrentQueue<int>();
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 16,
            WaitForMissingTimeout = TimeSpan.FromMilliseconds(80),
            BufferWaitTimeoutStrategy = BufferWaitTimeoutStrategy.SkipMissing,
        });

        BaseMessageQueue queue = provider((_, payload) =>
        {
            if (payload.TryGetProperty("v", out JsonElement p) && p.TryGetInt32(out int v))
                order.Enqueue(v);
            if (order.Count >= 2)
                tcs.TrySetResult();
            return Task.CompletedTask;
        });

        await queue.StartAsync();
        await queue.EnqueueAsync(CreatePayload(20), 2);
        await queue.EnqueueAsync(CreatePayload(40), 4);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await queue.StopAsync();

        Assert.Equal([20, 40], order.ToArray());
    }

    [Fact]
    public async Task When_WaitForMissingTimeout_null_never_fires_timeout_keeps_waiting()
    {
        var order = new ConcurrentQueue<int>();
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 16,
            WaitForMissingTimeout = null,
        });

        BaseMessageQueue queue = provider((_, payload) =>
        {
            if (payload.TryGetProperty("v", out JsonElement p) && p.TryGetInt32(out int v))
                order.Enqueue(v);
            if (order.Count >= 3)
                tcs.TrySetResult();
            return Task.CompletedTask;
        });

        await queue.StartAsync();
        await queue.EnqueueAsync(CreatePayload(20), 2);
        await queue.EnqueueAsync(CreatePayload(40), 4);
        await Task.Delay(120);
        Assert.Equal([20], order.ToArray());
        await queue.EnqueueAsync(CreatePayload(30), 3);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await queue.StopAsync();

        Assert.Equal([20, 30, 40], order.ToArray());
    }

    [Fact]
    public async Task When_WaitForMissingTimeout_InfiniteTimeSpan_never_fires_timeout_keeps_waiting()
    {
        var order = new ConcurrentQueue<int>();
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        MessageQueueProvider provider = InMemoryMessageQueueProvider.Create(new InMemoryMessageQueueOptions
        {
            EnableBuffering = true,
            BufferCapacity = 16,
            WaitForMissingTimeout = Timeout.InfiniteTimeSpan,
        });

        BaseMessageQueue queue = provider((_, payload) =>
        {
            if (payload.TryGetProperty("v", out JsonElement p) && p.TryGetInt32(out int v))
                order.Enqueue(v);
            if (order.Count >= 3)
                tcs.TrySetResult();
            return Task.CompletedTask;
        });

        await queue.StartAsync();
        await queue.EnqueueAsync(CreatePayload(20), 2);
        await queue.EnqueueAsync(CreatePayload(40), 4);
        await Task.Delay(120);
        Assert.Equal([20], order.ToArray());
        await queue.EnqueueAsync(CreatePayload(30), 3);
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await queue.StopAsync();

        Assert.Equal([20, 30, 40], order.ToArray());
    }
}
