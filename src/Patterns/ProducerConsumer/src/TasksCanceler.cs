// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gems.Patterns.ProducerConsumer
{
    internal class TasksCanceler
    {
        private readonly CancellationTokenSource[] cancellationTokenSources;

        public TasksCanceler(int length, CancellationToken linkedCancellationToken)
        {
            this.cancellationTokenSources = new CancellationTokenSource[length];
            for (var i = 0; i < length; i++)
            {
                this.cancellationTokenSources[i] = CancellationTokenSource.CreateLinkedTokenSource(linkedCancellationToken);
            }
        }

        public Exception Exception { get; private set; }

        public CancellationTokenSource this[int index] => this.cancellationTokenSources[index];

        public void CancelAllTasks(Task faultedTask)
        {
            if (!faultedTask.IsFaulted)
            {
                throw new ArgumentException("Task must be faulted.");
            }

            this.Exception = faultedTask.Exception?.InnerExceptions.FirstOrDefault() ?? faultedTask.Exception;

            foreach (var cancellationTokenSource in this.cancellationTokenSources)
            {
                cancellationTokenSource.Cancel();
            }
        }

        public virtual void ApplyCancelFunction(Task[] tasks)
        {
            foreach (var task in tasks)
            {
                task.ContinueWith(this.CancelAllTasks, TaskContinuationOptions.OnlyOnFaulted);
                task.ContinueWith(NoOperation, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }

        private static void NoOperation(Task task)
        {
        }
    }
}
