using System;
using System.Collections.Generic;
/***using System.Diagnostics.CodeAnalysis;***/
using System.Threading;
using System.Threading.Tasks;

namespace /***$rootnamespace$.***/ULibs.ConcurrentForEach
{
    /***[ExcludeFromCodeCoverage]***/
    internal static class Concurrent
    {
        /// <summary>
        /// Applies an asynchronous operation to each element in a sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="source">The input sequence of elements.</param>
        /// <param name="func">The asynchronous operation applied to each element in the sequence. It's strongly
        /// recommended that the operation should take care to handle its own expected exceptions.</param>
        /// <param name="maxConcurrentTasks">
        /// <para>
        /// The maximum number of concurrent operations. Must be greater than or equal to 1.
        /// </para>
        /// <para>
        /// The number of simultaneous operations can vary depending on the use case. For cpu intensive
        /// operations, consider using <see cref="Environment.ProcessorCount">Environment.ProcessorCount</see>.
        /// For operations that invoke the same web service for each item, RFC 7230 suggests that the number
        /// of simultaneous requests/connections should be limited (https://tools.ietf.org/html/rfc7230#section-6.4).
        /// A search for the connection limits used by common web-browsers suggests that a value in the range 6-8 is
        /// appropriate (any more, and you risk triggering abuse detection mechanisms). For operations that invoke a
        /// different web service for each item, a search for the connection limits used by common web-browsers
        /// suggests that a value in the range 10-20 is appropriate.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">Used to cancel the operations.</param>
        /// <returns>A task that can be awaited upon for all operations to complete. Awaiting on the task will
        /// raise an <see cref="AggregateException"/> if any operation fails, or work is cancelled via the
        /// <paramref name="cancellationToken"/>.</returns>
        public static Task ForEachAsync<T>(
            this IEnumerable<T> source,
            Func<T, Task> func,
            int maxConcurrentTasks,
            CancellationToken cancellationToken)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            return source.ForEachAsync((item, _) => func(item), maxConcurrentTasks, cancellationToken);
        }

        /// <summary>
        /// Applies an asynchronous operation to each element in a sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="source">The input sequence of elements.</param>
        /// <param name="func">The asynchronous operation applied to each element in the sequence. It's strongly
        /// recommended that the operation should take care to handle its own expected exceptions.</param>
        /// <param name="maxConcurrentTasks">
        /// <para>
        /// The maximum number of concurrent operations. Must be greater than or equal to 1.
        /// </para>
        /// <para>
        /// The number of simultaneous operations can vary depending on the use case. For cpu intensive
        /// operations, consider using <see cref="Environment.ProcessorCount">Environment.ProcessorCount</see>.
        /// For operations that invoke the same web service for each item, RFC 7230 suggests that the number
        /// of simultaneous requests/connections should be limited (https://tools.ietf.org/html/rfc7230#section-6.4).
        /// A search for the connection limits used by common web-browsers suggests that a value in the range 6-8 is
        /// appropriate (any more, and you risk triggering abuse detection mechanisms). For operations that invoke a
        /// different web service for each item, a search for the connection limits used by common web-browsers
        /// suggests that a value in the range 10-20 is appropriate.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">Used to cancel the operations.</param>
        /// <returns>A task that can be awaited upon for all operations to complete. Awaiting on the task will
        /// raise an <see cref="AggregateException"/> if any operation fails, or work is cancelled via the
        /// <paramref name="cancellationToken"/>.</returns>
        public static async Task ForEachAsync<T>(
            this IEnumerable<T> source,
            Func<T, CancellationToken, Task> func,
            int maxConcurrentTasks,
            CancellationToken cancellationToken)
        {
            if (maxConcurrentTasks < 1)
                throw new ArgumentException("Value cannot be less than 1", nameof(maxConcurrentTasks));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (func == null) throw new ArgumentNullException(nameof(func));

            using (var semaphore = new SemaphoreSlim(maxConcurrentTasks, maxConcurrentTasks))
            {
                var tasks = new List<Task>();
                foreach (var item in source)
                {
                    // Wait for the next available slot.
                    try
                    {
                        await semaphore.WaitAsync(cancellationToken);
                    }
                    catch (OperationCanceledException exception)
                    {
                        tasks.Add(Task.FromException(exception));
                        break;
                    }

                    // Discard completed tasks. Not strictly necessary, but keeps the list size down.
                    tasks.RemoveAll(task => task.IsCompleted);

                    // Kick-off the next task.
                    tasks.Add(CreateTask(func, item, cancellationToken).ReleaseSemaphoreOnCompletion(semaphore));
                }

                await Task.WhenAll(tasks);
            }
        }

        private static Task CreateTask<T>(
            Func<T, CancellationToken, Task> func, T item, CancellationToken cancellationToken)
        {
            try
            {
                return func(item, cancellationToken);
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        private static async Task ReleaseSemaphoreOnCompletion(this Task task, SemaphoreSlim semaphore)
        {
            try
            {
                await task;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}