using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ULibs.ConcurrentForEach;

namespace Ulibs.Tests.ConcurrentForEach
{
    [TestFixture]
    public class ConcurrentTests
    {
        private readonly Random _random = new Random();

        private int GetRandomDelay()
        {
            lock (_random)
            {
                return 50 + _random.Next(50);
            }
        }

        [Test]
        public void ForEachAsync_ArgChecks()
        {
            IEnumerable<int> sequence = Enumerable.Range(0, 10);
            IEnumerable<int> nullSequence = null;

            Task Operation(int i) => Task.CompletedTask;
            Func<int, Task> nullOperation = null;

            Task CancellableOperation(int i, CancellationToken t) => Task.CompletedTask;
            Func<int, CancellationToken, Task> nullCancellableOperation = null;

            Assert.That(() => sequence.ForEachAsync(nullOperation, 1, CancellationToken.None), Throws.ArgumentNullException);
            Assert.That(() => sequence.ForEachAsync(nullCancellableOperation, 1, CancellationToken.None), Throws.ArgumentNullException);
            Assert.That(() => nullSequence.ForEachAsync(Operation, 1, CancellationToken.None), Throws.ArgumentNullException);
            Assert.That(() => nullSequence.ForEachAsync(CancellableOperation, 1, CancellationToken.None), Throws.ArgumentNullException);
            Assert.That(() => sequence.ForEachAsync(Operation, 0, CancellationToken.None), Throws.ArgumentException);
            Assert.That(() => sequence.ForEachAsync(CancellableOperation, 0, CancellationToken.None), Throws.ArgumentException);
        }

        [Test]
        public void ForEachAsync_ExecutesTheOperationAgainstEachElement()
        {
            // ARRANGE
            var count = 0;
            Task Operation(int value) => Task.Run(() => Interlocked.Increment(ref count));
            var sequence = Enumerable.Range(0, 10);

            // ACT
            var task = sequence.ForEachAsync(Operation, 2, CancellationToken.None);

            // ASSERT
            Assert.That(task.Wait(2000), Is.True);
            Assert.That(count, Is.EqualTo(10));
        }

        [Test]
        public void ForEachAsync_LimitsTheNumberOfSimultaneousOperations()
        {
            // ARRANGE
            var currentOperationCount = 0;
            var maxOperationCount = 0;
            var monitor = new object();
            async Task Operation(int value)
            {
                lock (monitor)
                {
                    currentOperationCount++;
                    maxOperationCount = Math.Max(maxOperationCount, currentOperationCount);
                }
                await Task.Delay(GetRandomDelay());
                lock (monitor)
                {
                    currentOperationCount--;
                }
            }
            var sequence = Enumerable.Range(0, 10);

            // ACT
            var task = sequence.ForEachAsync(Operation, 2, CancellationToken.None);

            // ASSERT
            Assert.That(task.Wait(2000), Is.True);
            Assert.That(maxOperationCount, Is.EqualTo(2));
        }

        [Test]
        public void ForEachAsync_ExecutesTheOperationAgainstAllElements_InSpiteOfExceptions_Async()
        {
            // ARRANGE
            var count = 0;
            async Task Operation(int value)
            {
                Interlocked.Increment(ref count);
                if (value % 5 == 0)
                {
                    // Sometimes the func should fail immediately.
                    throw new ApplicationException("Error");
                }
                await Task.Delay(GetRandomDelay());
                if (value % 2 == 0)
                {
                    // Sometimes the func should fail after the first await.
                    throw new ApplicationException("Error");
                }
            }
            var sequence = Enumerable.Range(0, 10);

            // ACT
            var task = sequence.ForEachAsync(Operation, 2, CancellationToken.None);

            // ASSERT
            Assert.That(() => Assert.That(task.Wait(2000), Is.True),
                        Throws.InstanceOf<AggregateException>());
            Assert.That(count, Is.EqualTo(10));
        }

        [Test]
        public void ForEachAsync_ExecutesTheOperationAgainstAllElements_InSpiteOfExceptions_Sync()
        {
            // ARRANGE
            var count = 0;
            Task Operation(int value)
            {
                Interlocked.Increment(ref count);
                if (value % 5 == 0)
                {
                    // Sometimes the func should fail immediately.
                    throw new ApplicationException("Error");
                }

                return Task.Run(async () =>
                {
                    await Task.Delay(GetRandomDelay());
                    if (value % 2 == 0)
                    {
                        // Sometimes the func should fail after the first await.
                        throw new ApplicationException("Error");
                    }
                });
            }
            var sequence = Enumerable.Range(0, 10);

            // ACT
            var task = sequence.ForEachAsync(Operation, 2, CancellationToken.None);

            // ASSERT
            Assert.That(() => Assert.That(task.Wait(2000), Is.True),
                        Throws.InstanceOf<AggregateException>());
            Assert.That(count, Is.EqualTo(10));
        }

        [Test]
        public void ForEachAsync_AbortsWhenTheCancellationTokenIsCancelled_WhereEachTaskReceivesTheCancellationSignal()
        {
            // ARRANGE
            var count = 0;
            Task Operation(int value, CancellationToken token) => Task.Run(async () =>
            {
                Interlocked.Increment(ref count);
                await Task.Delay(GetRandomDelay(), token);
            });
            var sequence = Enumerable.Range(0, 10);
            var cancellationTokenSource = new CancellationTokenSource(200);
            var cancellationToken = cancellationTokenSource.Token;

            // ACT
            var task = sequence.ForEachAsync(Operation, 2, cancellationToken);

            // ASSERT
            Assert.That(() => Assert.That(task.Wait(2000), Is.True),
                        Throws.InstanceOf<AggregateException>());
            Assert.That(count, Is.GreaterThan(0).And.LessThan(10));
        }

        [Test]
        public void ForEachAsync_AbortsWhenTheCancellationTokenIsCancelled_WhereNoTaskReceivesTheCancellationSignal()
        {
            // ARRANGE
            var count = 0;
            Task Operation(int value) => Task.Run(async () =>
            {
                Interlocked.Increment(ref count);
                await Task.Delay(GetRandomDelay());
            });
            var sequence = Enumerable.Range(0, 10);
            var cancellationTokenSource = new CancellationTokenSource(200);
            var cancellationToken = cancellationTokenSource.Token;

            // ACT
            var task = sequence.ForEachAsync(Operation, 2, cancellationToken);

            // ASSERT
            Assert.That(() => Assert.That(task.Wait(2000), Is.True),
                        Throws.InstanceOf<AggregateException>());
            Assert.That(count, Is.GreaterThan(0).And.LessThan(10));
        }
    }
}