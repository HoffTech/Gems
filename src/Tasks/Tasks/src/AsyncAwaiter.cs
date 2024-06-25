// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gems.Tasks
{
    /// <summary>
    /// Добавляет возможность безопасно ждать завершения задач, требующих ограниченного доступа
    /// Использует безопасный семафор, чтобы предотвратить любую возможность тупиковой ситуации.
    /// </summary>
    public class AsyncAwaiter
    {
        /// <summary>
        /// Семафор для захвата списка семафоров.
        /// </summary>
        private static readonly SemaphoreSlim SelfLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Список семафоров.
        /// </summary>
        private static readonly Dictionary<string, SemaphoreSlim> Semaphores = new Dictionary<string, SemaphoreSlim>();

        /// <summary>
        /// Ожидает завершения любых невыполненных задач, которые обращаются к одному и тому же ключу, затем запускают данную задачу, возвращая её значение.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <param name="key">ключ для ожидания.</param>
        /// <param name="task">задача для выполнения внутри блокировки семафора.</param>
        /// <param name="maxAccessCount">если это первый вызов, то устанавливает максимальное количество задач, которые могут получить доступ к этой задаче.</param>
        /// <returns>Task.</returns>
        public static async Task<T> AwaitResultAsync<T>(string key, Func<Task<T>> task, int maxAccessCount = 1)
        {
            // Асинхронно ждём входа в семафор, если никому не предоставлен доступ к семафору, то выполнение продолжится
            // В противном случае этот поток ждёт здесь, пока семафор не будет освобождён.
            await SelfLock.WaitAsync();

            try
            {
                if (Semaphores.ContainsKey(key))
                {
                    Semaphores.Add(key, new SemaphoreSlim(maxAccessCount, maxAccessCount));
                }
            }
            finally
            {
                SelfLock.Release();
            }

            // Используем этот семафор и выполняем желаемую задачу внутри его блокировки.
            var semaphore = Semaphores[key];

            await semaphore.WaitAsync();

            try
            {
                return await task();
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Ожидает завершения любых невыполненных задач, которые обращаются к одному и тому же ключу, затем запускает данную задачу.
        /// </summary>
        /// <param name="key">ключ для ожидания.</param>
        /// <param name="task">задача для выполнения внутри блокировки семафора.</param>
        /// <param name="maxAccessCount">если это первый вызов, то устанавливает максимальное количество задач, которые могут получить доступ к этой задаче.</param>
        /// <returns>Task.</returns>
        public static async Task AwaitAsync(string key, Func<Task> task, int maxAccessCount = 1)
        {
            // Асинхронно ждём входа в семафор, если никому не предоставлен доступ к семафору, то выполнение продолжится
            // В противном случае этот поток ждёт здесь, пока семафор не будет освобождён.
            await SelfLock.WaitAsync();

            try
            {
                if (Semaphores.ContainsKey(key))
                {
                    Semaphores.Add(key, new SemaphoreSlim(maxAccessCount, maxAccessCount));
                }
            }
            finally
            {
                SelfLock.Release();
            }

            // Используем этот семафор и выполняем желаемую задачу внутри его блокировки.
            var semaphore = Semaphores[key];

            await semaphore.WaitAsync();

            try
            {
                await task();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
