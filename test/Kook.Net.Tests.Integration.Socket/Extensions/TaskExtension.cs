using System;
using System.Threading.Tasks;

namespace Kook;

/// <summary>
///     Adds extension methods to the <see cref="Task"/> class.
/// </summary>
public static class TaskExtension
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);

    /// <summary>
    ///     Waits for the task to complete within the specified timeout.
    /// </summary>
    /// <param name="task"> The task to wait for. </param>
    /// <returns> The task. </returns>
    /// <exception cref="TimeoutException"> The task did not complete within the specified timeout. </exception>
    public static async Task WithTimeout(this Task task) => await task.WithTimeout(DefaultTimeout);

    /// <summary>
    ///     Waits for the task to complete within the specified timeout.
    /// </summary>
    /// <param name="task"> The task to wait for. </param>
    /// <param name="timeout"> A <see cref="TimeSpan"/> representing the timeout duration. </param>
    /// <returns> The task. </returns>
    /// <exception cref="TimeoutException"> The task did not complete within the specified timeout. </exception>
    public static async Task WithTimeout(this Task task, TimeSpan timeout)
    {
        Task delayTask = Task.Delay(timeout);
        Task completedTask = await Task.WhenAny(task, delayTask);
        if (completedTask == delayTask)
            throw new TimeoutException();
        await task;
    }

    /// <summary>
    ///     Waits for the task to complete within the specified timeout.
    /// </summary>
    /// <param name="task"> The task to wait for. </param>
    /// <typeparam name="T"> The type of the task result. </typeparam>
    /// <returns> The result of the task. </returns>
    /// <exception cref="TimeoutException"> The task did not complete within the specified timeout. </exception>
    public static Task<T> WithTimeout<T>(this Task<T> task) => WithTimeout(task, DefaultTimeout);

    /// <summary>
    ///     Waits for the task to complete within the specified timeout.
    /// </summary>
    /// <param name="task"> The task to wait for. </param>
    /// <param name="timeout"> A <see cref="TimeSpan"/> representing the timeout duration. </param>
    /// <typeparam name="T"> The type of the task result. </typeparam>
    /// <returns> The result of the task. </returns>
    /// <exception cref="TimeoutException"> The task did not complete within the specified timeout. </exception>
    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
    {
        Task delayTask = Task.Delay(timeout);
        Task completedTask = await Task.WhenAny(task, delayTask);
        if (completedTask == delayTask)
            throw new TimeoutException();
        return await task;
    }
}
