using System.Collections.Immutable;

namespace Kook;

internal class AsyncEvent<T>
    where T : class
{
    private readonly Lock _subLock = new();
    internal ImmutableArray<T> _subscriptions = [];

    // ReSharper disable once InconsistentlySynchronizedField
    public bool HasSubscribers => _subscriptions.Length != 0;

    // ReSharper disable once InconsistentlySynchronizedField
    public IReadOnlyList<T> Subscriptions => _subscriptions;

    public void Add(T subscriber)
    {
        Preconditions.NotNull(subscriber, nameof(subscriber));
        lock (_subLock)
        {
            _subscriptions = _subscriptions.Add(subscriber);
        }
    }

    public void Remove(T subscriber)
    {
        Preconditions.NotNull(subscriber, nameof(subscriber));
        lock (_subLock)
        {
            _subscriptions = _subscriptions.Remove(subscriber);
        }
    }
}

internal static class EventExtensions
{
    public static async Task InvokeAsync(this AsyncEvent<Func<Task>> eventHandler)
    {
        IReadOnlyList<Func<Task>> subscribers = eventHandler.Subscriptions;
        foreach (Func<Task> x in subscribers)
            await x.Invoke().ConfigureAwait(false);
    }

    public static async Task InvokeAsync<T>(this AsyncEvent<Func<T, Task>> eventHandler, T arg)
    {
        IReadOnlyList<Func<T, Task>> subscribers = eventHandler.Subscriptions;
        foreach (Func<T, Task> x in subscribers)
            await x.Invoke(arg).ConfigureAwait(false);
    }

    public static async Task InvokeAsync<T1, T2>(this AsyncEvent<Func<T1, T2, Task>> eventHandler, T1 arg1, T2 arg2)
    {
        IReadOnlyList<Func<T1, T2, Task>> subscribers = eventHandler.Subscriptions;
        foreach (Func<T1, T2, Task> x in subscribers)
            await x.Invoke(arg1, arg2).ConfigureAwait(false);
    }

    public static async Task InvokeAsync<T1, T2, T3>(this AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3)
    {
        IReadOnlyList<Func<T1, T2, T3, Task>> subscribers = eventHandler.Subscriptions;
        foreach (Func<T1, T2, T3, Task> x in subscribers)
            await x.Invoke(arg1, arg2, arg3).ConfigureAwait(false);
    }

    public static async Task InvokeAsync<T1, T2, T3, T4>(this AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        IReadOnlyList<Func<T1, T2, T3, T4, Task>> subscribers = eventHandler.Subscriptions;
        foreach (Func<T1, T2, T3, T4, Task> x in subscribers)
            await x.Invoke(arg1, arg2, arg3, arg4).ConfigureAwait(false);
    }

    public static async Task InvokeAsync<T1, T2, T3, T4, T5>(this AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        IReadOnlyList<Func<T1, T2, T3, T4, T5, Task>> subscribers = eventHandler.Subscriptions;
        foreach (Func<T1, T2, T3, T4, T5, Task> x in subscribers)
            await x.Invoke(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
    }
}
