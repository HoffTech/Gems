// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Http
{
    /// <summary>
    /// Опции для отправки запросов.
    /// </summary>
    public class HttpClientServiceOptions
    {
        /// <summary>
        ///  Начиная с какого http статуса делать повторные запросы в случае ошибки. По умолчанию 499.
        /// </summary>
        public int DurableFromHttpStatus { get; set; } = 499;

        /// <summary>
        /// true - запрос будет повторяться указанное количество раз в случае ошибки.
        /// </summary>
        public bool Durable { get; set; }

        /// <summary>
        /// Количество повторных попыток в случае если отправка запроса не удалась.
        /// </summary>
        public short Attempts { get; set; } = 3;

        /// <summary>
        /// Количество милисекунд задержки перед повторной отправкой.
        /// </summary>
        public int MillisecondsDelay { get; set; } = 5000;

        /// <summary>
        /// Время ожидания в милисекундах для выполнения запроса.
        /// </summary>
        public int RequestTimeout { get; set; }

        /// <summary>
        /// Необходимо загрузить сертификат.
        /// </summary>
        public bool NeedDownloadCertificate { get; set; }

        /// <summary>
        /// Базовый url сервиса.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Тип метрик для кодов Http.
        /// </summary>
        public Enum StatusCodeMetricType { get; set; }
    }
}
