// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gems.Tasks
{
    /// <summary>
    /// Добавляет возможность итеративно распараллелить выполнение задач по обработке массива данных.
    /// </summary>
    public class ParallelExecutor
    {
        /// <summary>
        /// Параллельное выполнение синхронизации данных. Перенос данных из источника в приёмник.
        /// </summary>
        /// <param name="totalItems">Общее колличество элементов для обработки.</param>
        /// <param name="maxSemaphoreTasks">Ограничение на максимальное колличество одновременно выполненяемых.</param>
        /// <param name="exceptionHandleTypes">Список исключений, по которым будут осуществляться повторные попытки обработки.</param>
        /// <param name="action">Действие для задачи (skip, take, CancellationToken).</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <param name="maxTakeSize">Ограничение на максимальное колличество строк для выборки.</param>
        /// <param name="maxAttempts">Ограничение на максимальное колличество попыток связанных с БД при возникновении ошибок при синхронизации.</param>
        public static async Task SyncDataAsync(
            int totalItems,
            int maxSemaphoreTasks,
            List<Type> exceptionHandleTypes,
            Func<int, int, CancellationToken, Task> action,
            CancellationToken cancellationToken,
            int maxTakeSize = 1000,
            int maxAttempts = 10)
        {
            var key = Guid.NewGuid().ToString();

            // формируем массив действий для задач
            var actionsCount = CalculateActionsCount(totalItems, maxTakeSize);
            var actions = new Func<int, int, CancellationToken, Task>[actionsCount];
            for (var i = 0; i < actionsCount; i++)
            {
                actions[i] = action;
            }

            // формируем параллельные задачи с учетом ограничения по максимальному кол-ву задач на параллельное выполнение в семафоре
            var tasks = actions.Select(
                (func, taskNumber) =>
                {
                    var skip = CalculateSkipSize(taskNumber, totalItems, actionsCount, maxTakeSize);
                    var take = CalculateTakeSize(totalItems, skip, maxTakeSize);

                    return AsyncAwaiter.AwaitAsync(
                        key,
                        async () =>
                        {
                            await AsyncDecorator
                                .DurableExecuteAsync(
                                    async token =>
                                    {
                                        await func(skip, take, token).ConfigureAwait(false);
                                        return Task.CompletedTask;
                                    },
                                    cancellationToken,
                                    TimeSpan.Zero,
                                    maxAttempts,
                                    exceptionHandleTypes)
                                .ConfigureAwait(false);
                        },
                        maxSemaphoreTasks);
                });

            // запускаем сформированный пакет задач на выполнение и ожидаем завершения
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Расчет кол-ва задач для параллельной синхронизации.
        /// </summary>
        /// <param name="totalRows">Общее колличество строк.</param>
        /// <param name="maxTakeSize">Максимальное колличество строк для выборки.</param>
        /// <returns>колличество задач для параллельной синхронизации.</returns>
        private static int CalculateActionsCount(int totalRows, int maxTakeSize)
        {
            return (int)Math.Ceiling((double)totalRows / maxTakeSize);
        }

        /// <summary>
        /// Расчет кол-ва строк для пропуска.
        /// </summary>
        /// <param name="taskNumber">Номер текущей задачи.</param>
        /// <param name="totalRows">Общее колличество строк.</param>
        /// <param name="actionsCount">Общее колличество задач.</param>
        /// <param name="maxTakeSize">Максимальное колличество строк для выборки.</param>
        /// <returns>колличество строк для пропуска.</returns>
        private static int CalculateSkipSize(int taskNumber, int totalRows, int actionsCount, int maxTakeSize)
        {
            // рассчитываем колличество строк для пропуска по формуле i*N/T,
            // где:
            // i - номер текущей задачи,
            // N - общее колличество строк
            // T - общее колличество задач
            var skip = (double)taskNumber * totalRows / actionsCount;

            // округляем на величину maxTakeSize, учитывая остаток от деления
            return (int)Math.Ceiling(skip / maxTakeSize) * maxTakeSize;
        }

        /// <summary>
        /// Расчет кол-ва строк для выборки с учетом оставшихся строк.
        /// </summary>
        /// <param name="totalRows">Общее колличество строк.</param>
        /// <param name="skip">колличество строк для пропуска.</param>
        /// <param name="maxTakeSize">колличество строк для выборки в рамках одной задачи.</param>
        /// <returns>колличество строк для выборки.</returns>
        private static int CalculateTakeSize(int totalRows, int skip, int maxTakeSize)
        {
            var rowsRemaining = totalRows - skip;
            return rowsRemaining < maxTakeSize ? rowsRemaining : maxTakeSize;
        }
    }
}
