// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Tasks;

using Microsoft.Extensions.Options;

namespace Gems.Patterns.ProducerConsumer
{
    public class ProducerConsumer<TTaskInfo>
    {
        private readonly IOptions<ProducerConsumerOptions> options;
        private readonly Func<ProducerConsumer<TTaskInfo>, CancellationToken, Task> produceInternalAsync;
        private readonly Func<TTaskInfo, CancellationToken, Task> consumeInternalAsync;
        private readonly List<Type> exceptionHandleTypes;
        private readonly Func<Exception, Task> onErrorAction;
        private BlockingCollection<TTaskInfo> taskInfos;

        public ProducerConsumer(
            IOptions<ProducerConsumerOptions> options,
            Func<ProducerConsumer<TTaskInfo>, CancellationToken, Task> produceInternalAsync,
            Func<TTaskInfo, CancellationToken, Task> consumeInternalAsync,
            Func<Exception, Task> onErrorAction = null,
            List<Type> exceptionHandleTypes = null)
        {
            this.options = options;
            this.produceInternalAsync = produceInternalAsync;
            this.consumeInternalAsync = consumeInternalAsync;
            this.onErrorAction = onErrorAction;
            this.exceptionHandleTypes = exceptionHandleTypes;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this.StartAsync(0, cancellationToken);
        }

        public async Task StartAsync(int consumerCount, CancellationToken cancellationToken)
        {
            if (this.taskInfos != null)
            {
                return;
            }

            const int producerTasksCount = 1;
            const int producerTaskIndex = 0;

            if (consumerCount <= 0)
            {
                consumerCount = Environment.ProcessorCount <= 1 ? 1 : Environment.ProcessorCount - 1;
            }

            this.taskInfos = new BlockingCollection<TTaskInfo>(consumerCount);

            var tasks = new Task[producerTasksCount + consumerCount];
            var tasksCanceler = new TasksCanceler(tasks.Length, cancellationToken);

            try
            {
                tasks[producerTaskIndex] = this.ProduceAsync(tasksCanceler[producerTaskIndex].Token);
                for (var i = producerTasksCount; i < tasks.Length; i++)
                {
                    tasks[i] = this.StartConsumeAsync(tasksCanceler[i].Token);
                }

                tasksCanceler.ApplyCancelFunction(tasks);
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (TaskCanceledException e)
            {
                throw tasksCanceler.Exception ?? e;
            }
            finally
            {
                this.taskInfos.Dispose();
            }
        }

        public void AddTaskInfo(TTaskInfo taskInfo)
        {
            this.taskInfos.Add(taskInfo);
        }

        private async Task StartConsumeAsync(CancellationToken cancellationToken)
        {
            foreach (var item in this.taskInfos.GetConsumingEnumerable(cancellationToken))
            {
                await this.Consume(item, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task ProduceAsync(CancellationToken cancellationToken)
        {
            try
            {
                await this.produceInternalAsync(this, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                this.taskInfos.CompleteAdding();
            }
        }

        private Task Consume(TTaskInfo taskInfo, CancellationToken cancellationToken)
        {
            return AsyncDecorator
                .DurableExecuteAsync(
                    async token =>
                    {
                        await this.consumeInternalAsync(taskInfo, cancellationToken).ConfigureAwait(false);
                        return Task.CompletedTask;
                    },
                    cancellationToken,
                    TimeSpan.FromMilliseconds(this.options.Value.DelayBetweenAttemptsInMilliseconds),
                    this.options.Value.MaxAttempts,
                    this.exceptionHandleTypes,
                    this.onErrorAction);
        }
    }
}
