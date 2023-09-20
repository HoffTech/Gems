// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Jobs
{
    public interface IRequestReFireJobOnFailed
    {
        /// <summary>
        /// Получение промежутка времени для задержки в случае ошибки во время выполнения задачи.
        /// </summary>
        /// <returns>Промежуток времени для задержки между повторными попытками.</returns>
        public TimeSpan GetReFireJobOnErrorDelay()
        {
            return TimeSpan.FromMilliseconds(10_000);
        }
    }
}
