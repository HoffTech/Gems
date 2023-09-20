// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gems.Tasks
{
    /// <summary>
    /// Содержит декораторы с различными сценариями для асинхронных методов.
    /// </summary>
    public static class AsyncDecorator
    {
        /// <summary>
        /// Метод декорирующий асинхронную задачу, учитывающий максимальное кол-во попыток на выполнение и пользовательскую обработку в случае возникновения исключений.
        /// </summary>
        /// <param name="action">Действие для задачи.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <param name="delayBetweenAttempts">Временной интервал между попытками.</param>
        /// <param name="maxAttempts">Ограничение на максимальное колличество попыток выполнения задачи.</param>
        /// <param name="exceptionHandleTypes">Список исключений, по которым будут осуществляться повторные попытки синхронизации.</param>
        /// <param name="onErrorAction">Действие в случае возникновения ошибки.</param>
        /// <typeparam name="TReturn">Возвращаемый результат задачи.</typeparam>
        /// <returns>Результат задачи.</returns>
        public static async Task<TReturn> DurableExecuteAsync<TReturn>(
            Func<CancellationToken, Task<TReturn>> action,
            CancellationToken cancellationToken,
            TimeSpan delayBetweenAttempts = default,
            int maxAttempts = 10,
            List<Type> exceptionHandleTypes = null,
            Func<Exception, Task> onErrorAction = null)
        {
            var currentAttempt = 1;
            while (true)
            {
                try
                {
                    return await action(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex) when (exceptionHandleTypes?.Contains(ex.GetType()) ?? true)
                {
                    if (onErrorAction != null)
                    {
                        await onErrorAction(ex).ConfigureAwait(false);
                    }

                    currentAttempt++;
                    if (currentAttempt == maxAttempts)
                    {
                        throw;
                    }

                    if (delayBetweenAttempts != default)
                    {
                        await Task.Delay(delayBetweenAttempts, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
