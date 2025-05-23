// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Gems.Http.Serializers;
using Gems.Logging.Mvc.LogsCollector;
using Gems.Metrics;
using Gems.Metrics.Http;
using Gems.Mvc;
using Gems.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.Http
{
    public abstract class BaseClientService<TDefaultError>
    {
        private readonly IOptions<HttpClientServiceOptions> options;
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IMetricsService metricsService;
        private readonly IRequestLogsCollectorFactory logsCollectorFactory;
        private readonly IOptions<RequestLogsCollectorOptions> requestLogsCollectorOptions;
        private string baseUrl;

        protected BaseClientService(IOptions<HttpClientServiceOptions> options, BaseClientServiceHelper helper)
        {
            this.options = options;
            this.logger = helper.Logger;
            this.metricsService = helper.MetricsService;
            this.logsCollectorFactory = helper.LogsCollectorFactory;
            this.requestLogsCollectorOptions = helper.RequestLogsCollectorOptions;
            this.httpClientFactory = helper.HttpClientFactory;
            this.SetBaseUrl(helper.Configuration);
        }

        /// <summary>
        /// Получает последнее исключение, которое возникло при вызове метода TrySendRequestAsync.
        /// </summary>
        public Exception LastException { get; private set; }

        /// <summary>
        /// Опция для указания логировать запрос или нет.
        /// </summary>
        protected virtual bool LogRequest => this.options?.Value?.LogRequest ?? true;

        /// <summary>
        /// MediaType по умолчанию.
        /// </summary>
        protected virtual string MediaType => "application/json";

        /// <summary>
        /// Базовый url сервиса.
        /// </summary>
        protected virtual string BaseUrl => this.baseUrl;

        /// <summary>
        /// Начиная с какого http статуса делать повторные запросы в случае ошибки. По умолчанию 499.
        /// Переопределяет значение из настроек.
        /// </summary>
        protected virtual int DurableFromHttpStatus => this.options?.Value?.DurableFromHttpStatus ?? 499;

        /// <summary>
        /// true - запрос будет повторяться указанное количество раз в случае ошибки.
        /// Переопределяет значение из настроек.
        /// </summary>
        protected virtual bool Durable => this.options?.Value?.Durable ?? false;

        /// <summary>
        /// Количество повторных попыток в случае если отправка запроса не удалась.
        /// Переопределяет значение из настроек.
        /// </summary>
        protected virtual short Attempts => this.options?.Value?.Attempts ?? 3;

        /// <summary>
        /// Количество милисекунд задержки перед повторной отправкой.
        /// Переопределяет значение из настроек.
        /// </summary>
        protected virtual int MillisecondsDelay => this.options?.Value?.MillisecondsDelay ?? 5000;

        /// <summary>
        /// Время ожидания в милисекундах для выполнения запроса.
        /// Переопределяет значение из настроек.
        /// </summary>
        protected virtual int RequestTimeout => this.options?.Value?.RequestTimeout ?? 0;

        /// <summary>
        /// Необходимо загрузить сертификат.
        /// </summary>
        protected virtual bool NeedDownloadCertificate => this.options?.Value?.NeedDownloadCertificate ?? false;

        /// <summary>
        /// Конвертеры для десериализации в json.
        /// </summary>
        protected virtual IList<JsonConverter> DeserializeAdditionalConverters => null;

        /// <summary>
        /// Конвертеры для сериализации из json.
        /// </summary>
        protected virtual IList<JsonConverter> SerializeAdditionalConverters => null;

        /// <summary>
        /// true - сериализовать в Camel Case.
        /// </summary>
        protected virtual bool IsCamelCase => true;

        /// <summary>
        /// DefaultBoundary для multipart/form-data.
        /// </summary>
        protected virtual string DefaultBoundary => "-------------------Boundary--";

        /// <summary>
        /// Дополнительные скалярные структуры для сериализации.
        /// Например DateTime, DateOnly, Guid, DateTimeOffset.
        /// </summary>
        protected virtual Type[] AdditionalScalarStructureTypes => null;

        /// <summary>
        /// Уровни ошибок по статусам или группам статусов 200, 400 и 500.
        /// </summary>
        protected virtual List<LogLevelOptions> LogLevelsByHttpStatus => null;

        /// <summary>
        /// Bearer Токен.
        /// </summary>
        private string AccessToken { get; set; }

        public Task<(TResponse, TError)> TrySendRequestAsync<TResponse, TError>(
            HttpMethod httpMethod,
            string requestUri,
            object requestData,
            IDictionary<string, string> headers,
            CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(
                httpMethod,
                requestUri.ToTemplateUri(),
                requestData,
                headers,
                false,
                cancellationToken);
        }

        public virtual Task<TResponse> SendRequestAsync<TResponse, TError>(
            HttpMethod httpMethod,
            string requestUri,
            object requestData,
            IDictionary<string, string> headers,
            CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(
                httpMethod,
                requestUri.ToTemplateUri(),
                requestData,
                headers,
                false,
                cancellationToken);
        }

        public virtual Task<TResponse> SendRequestAsync<TResponse, TError>(
            HttpMethod httpMethod,
            string requestUri,
            object requestData,
            IDictionary<string, string> headers,
            bool isAuthenticationRequest,
            CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(
                httpMethod,
                requestUri.ToTemplateUri(),
                requestData,
                headers,
                isAuthenticationRequest,
                cancellationToken);
        }

        public virtual Task<(TResponse, TError)> TrySendRequestAsync<TResponse, TError>(
            HttpMethod httpMethod,
            string requestUri,
            object requestData,
            IDictionary<string, string> headers,
            bool isAuthenticationRequest,
            CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(
                httpMethod,
                requestUri.ToTemplateUri(),
                requestData,
                headers,
                isAuthenticationRequest,
                cancellationToken);
        }

        public Task<(TResponse, TError)> TrySendRequestAsync<TResponse, TError>(
            HttpMethod httpMethod,
            TemplateUri templateUri,
            object requestData,
            IDictionary<string, string> headers,
            CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(
                httpMethod,
                templateUri,
                requestData,
                headers,
                false,
                cancellationToken);
        }

        public virtual Task<TResponse> SendRequestAsync<TResponse, TError>(
            HttpMethod httpMethod,
            TemplateUri templateUri,
            object requestData,
            IDictionary<string, string> headers,
            CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(
                httpMethod,
                templateUri,
                requestData,
                headers,
                false,
                cancellationToken);
        }

        public virtual async Task<TResponse> SendRequestAsync<TResponse, TError>(
            HttpMethod httpMethod,
            TemplateUri templateUri,
            object requestData,
            IDictionary<string, string> headers,
            bool isAuthenticationRequest,
            CancellationToken cancellationToken)
        {
            Task<TResponse> SendRequestInnerAsync() => this.SendWithHandlingExceptionsAsync<TResponse, TError>(
                httpMethod,
                templateUri,
                requestData,
                headers,
                isAuthenticationRequest,
                cancellationToken);

            return this.Durable
                ? await this.DurableRequestAsync<TResponse, TError>(SendRequestInnerAsync, cancellationToken).ConfigureAwait(false)
                : await SendRequestInnerAsync().ConfigureAwait(false);
        }

        public virtual async Task<(TResponse, TError)> TrySendRequestAsync<TResponse, TError>(
            HttpMethod httpMethod,
            TemplateUri templateUri,
            object requestData,
            IDictionary<string, string> headers,
            bool isAuthenticationRequest,
            CancellationToken cancellationToken)
        {
            TResponse response = default;
            TError error = default;
            try
            {
                response = await this.SendRequestAsync<TResponse, TError>(httpMethod, templateUri, requestData, headers, isAuthenticationRequest, cancellationToken);
            }
            catch (RequestException<TError> e)
            {
                this.LastException = e;
                error = e.Error;
            }

            return (response, error);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> SendAuthenticationRequestAsync<TResponse>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, requestUri, requestData, headers, true, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TrySendAuthenticationRequestAsync<TResponse>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, requestUri, requestData, headers, true, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> GetWithCustomErrorAsync<TResponse, TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> GetWithCustomErrorAsync<TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<string> GetStringWithCustomErrorAsync<TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Stream> GetStreamWithCustomErrorAsync<TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<byte[]> GetByteArrayWithCustomErrorAsync<TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> GetWithCustomErrorAsync<TResponse, TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> GetWithCustomErrorAsync<TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<string> GetStringWithCustomErrorAsync<TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Stream> GetStreamWithCustomErrorAsync<TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<byte[]> GetByteArrayWithCustomErrorAsync<TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> DeleteWithCustomErrorAsync<TResponse, TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Delete, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> DeleteWithCustomErrorAsync<TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Delete, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> DeleteWithCustomErrorAsync<TResponse, TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Delete, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> DeleteWithCustomErrorAsync<TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Delete, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PostWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PostWithCustomErrorAsync<TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PutWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Put, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PutWithCustomErrorAsync<TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Put, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PatchWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Patch, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PatchWithCustomErrorAsync<TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> GetWithCustomErrorAsync<TResponse, TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> GetWithCustomErrorAsync<TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<string> GetStringWithCustomErrorAsync<TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Stream> GetStreamWithCustomErrorAsync<TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<byte[]> GetByteArrayWithCustomErrorAsync<TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> GetWithCustomErrorAsync<TResponse, TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> GetWithCustomErrorAsync<TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<string> GetStringWithCustomErrorAsync<TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Stream> GetStreamWithCustomErrorAsync<TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<byte[]> GetByteArrayWithCustomErrorAsync<TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> DeleteWithCustomErrorAsync<TResponse, TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Delete, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> DeleteWithCustomErrorAsync<TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Delete, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> DeleteWithCustomErrorAsync<TResponse, TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Delete, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> DeleteWithCustomErrorAsync<TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Delete, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PostWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PostWithCustomErrorAsync<TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PutWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Put, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PutWithCustomErrorAsync<TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Put, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PatchWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Patch, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PatchWithCustomErrorAsync<TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> GetAsync<TResponse>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> GetAsync(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<string> GetStringAsync(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TDefaultError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Stream> GetStreamAsync(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<byte[]> GetByteArrayAsync(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> GetAsync<TResponse>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> GetAsync(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<string> GetStringAsync(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TDefaultError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Stream> GetStreamAsync(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<byte[]> GetByteArrayAsync(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> DeleteAsync<TResponse>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> DeleteAsync(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> DeleteAsync<TResponse>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> DeleteAsync(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PostAsync<TResponse>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PostAsync(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PutAsync<TResponse>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Put, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PutAsync(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Put, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PatchAsync<TResponse>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Patch, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PatchAsync(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> GetAsync<TResponse>(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> GetAsync(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<string> GetStringAsync(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TDefaultError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Stream> GetStreamAsync(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<byte[]> GetByteArrayAsync(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> GetAsync<TResponse>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> GetAsync(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<string> GetStringAsync(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TDefaultError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Stream> GetStreamAsync(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<byte[]> GetByteArrayAsync(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> DeleteAsync<TResponse>(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> DeleteAsync(string requestUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> DeleteAsync<TResponse>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> DeleteAsync(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PostAsync<TResponse>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PostAsync(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PutAsync<TResponse>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Put, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PutAsync(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Put, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<TResponse> PatchAsync<TResponse>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Patch, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<Unit> PatchAsync(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryGetWithCustomErrorAsync<TResponse, TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryGetWithCustomErrorAsync<TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(string, TError)> TryGetStringWithCustomErrorAsync<TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Stream, TError)> TryGetStreamWithCustomErrorAsync<TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(byte[], TError)> TryGetByteArrayWithCustomErrorAsync<TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryGetWithCustomErrorAsync<TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(string, TError)> TryGetStringWithCustomErrorAsync<TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Stream, TError)> TryGetStreamWithCustomErrorAsync<TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(byte[], TError)> TryGetByteArrayWithCustomErrorAsync<TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryDeleteWithCustomErrorAsync<TResponse, TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Delete, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryDeleteWithCustomErrorAsync<TError>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Delete, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryDeleteWithCustomErrorAsync<TResponse, TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Delete, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryDeleteWithCustomErrorAsync<TError>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Delete, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryPostWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryPostWithCustomErrorAsync<TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryPutWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Put, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryPutWithCustomErrorAsync<TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Put, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryPatchWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Patch, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryPatchWithCustomErrorAsync<TError>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryGetWithCustomErrorAsync<TResponse, TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryGetWithCustomErrorAsync<TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(string, TError)> TryGetStringWithCustomErrorAsync<TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Stream, TError)> TryGetStreamWithCustomErrorAsync<TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(byte[], TError)> TryGetByteArrayWithCustomErrorAsync<TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryGetWithCustomErrorAsync<TResponse, TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryGetWithCustomErrorAsync<TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(string, TError)> TryGetStringWithCustomErrorAsync<TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Stream, TError)> TryGetStreamWithCustomErrorAsync<TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(byte[], TError)> TryGetByteArrayWithCustomErrorAsync<TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryDeleteWithCustomErrorAsync<TResponse, TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Delete, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryDeleteWithCustomErrorAsync<TError>(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Delete, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryDeleteWithCustomErrorAsync<TResponse, TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Delete, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryDeleteWithCustomErrorAsync<TError>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Delete, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryPostWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryPostWithCustomErrorAsync<TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryPutWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Put, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryPutWithCustomErrorAsync<TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Put, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TError)> TryPatchWithCustomErrorAsync<TResponse, TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Patch, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TError)> TryPatchWithCustomErrorAsync<TError>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryGetAsync<TResponse>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryGetAsync(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(string, TDefaultError)> TryGetStringAsync(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TDefaultError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Stream, TDefaultError)> TryGetStreamAsync(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(byte[], TDefaultError)> TryGetByteArrayAsync(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryGetAsync<TResponse>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryGetAsync(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(string, TDefaultError)> TryGetStringAsync(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TDefaultError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Stream, TDefaultError)> TryGetStreamAsync(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(byte[], TDefaultError)> TryGetByteArrayAsync(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryDeleteAsync<TResponse>(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryDeleteAsync(string requestUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, requestUri, null, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryDeleteAsync<TResponse>(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryDeleteAsync(string requestUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, requestUri, queryString, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryPostAsync<TResponse>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryPostAsync(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryPutAsync<TResponse>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Put, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryPutAsync(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Put, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryPatchAsync<TResponse>(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Patch, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryPatchAsync(string requestUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, requestUri, requestData, headers, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryGetAsync<TResponse>(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryGetAsync(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(string, TDefaultError)> TryGetStringAsync(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TDefaultError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Stream, TDefaultError)> TryGetStreamAsync(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(byte[], TDefaultError)> TryGetByteArrayAsync(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryGetAsync<TResponse>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryGetAsync(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(string, TDefaultError)> TryGetStringAsync(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TDefaultError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Stream, TDefaultError)> TryGetStreamAsync(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(byte[], TDefaultError)> TryGetByteArrayAsync(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryDeleteAsync<TResponse>(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryDeleteAsync(string requestUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, requestUri, null, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryDeleteAsync<TResponse>(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryDeleteAsync(string requestUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, requestUri, queryString, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryPostAsync<TResponse>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryPostAsync(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryPutAsync<TResponse>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Put, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryPutAsync(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Put, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(TResponse, TDefaultError)> TryPatchAsync<TResponse>(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Patch, requestUri, requestData, null, false, cancellationToken);
        }

        [Obsolete("Use overload method with TemplateUri.")]
        public Task<(Unit, TDefaultError)> TryPatchAsync(string requestUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, requestUri, requestData, null, false, cancellationToken);
        }

        public Task<TResponse> SendAuthenticationRequestAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, templateUri, requestData, headers, true, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TrySendAuthenticationRequestAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, templateUri, requestData, headers, true, cancellationToken);
        }

        public Task<TResponse> GetWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<Unit> GetWithCustomErrorAsync<TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<string> GetStringWithCustomErrorAsync<TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<Stream> GetStreamWithCustomErrorAsync<TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<byte[]> GetByteArrayWithCustomErrorAsync<TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<TResponse> GetWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<Unit> GetWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<string> GetStringWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<Stream> GetStreamWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<byte[]> GetByteArrayWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<TResponse> DeleteWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Delete, templateUri, null, headers, false, cancellationToken);
        }

        public Task<Unit> DeleteWithCustomErrorAsync<TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Delete, templateUri, null, headers, false, cancellationToken);
        }

        public Task<TResponse> DeleteWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Delete, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<Unit> DeleteWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Delete, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<TResponse> PostWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<Unit> PostWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<TResponse> PutWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Put, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<Unit> PutWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Put, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<TResponse> PatchWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Patch, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<Unit> PatchWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<TResponse> GetWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<Unit> GetWithCustomErrorAsync<TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<string> GetStringWithCustomErrorAsync<TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<Stream> GetStreamWithCustomErrorAsync<TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<byte[]> GetByteArrayWithCustomErrorAsync<TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<TResponse> GetWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<Unit> GetWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<string> GetStringWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<Stream> GetStreamWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<byte[]> GetByteArrayWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<TResponse> DeleteWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Delete, templateUri, null, null, false, cancellationToken);
        }

        public Task<Unit> DeleteWithCustomErrorAsync<TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Delete, templateUri, null, null, false, cancellationToken);
        }

        public Task<TResponse> DeleteWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Delete, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<Unit> DeleteWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Delete, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<TResponse> PostWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<Unit> PostWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<TResponse> PutWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Put, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<Unit> PutWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Put, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<TResponse> PatchWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TError>(HttpMethod.Patch, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<Unit> PatchWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<TResponse> GetAsync<TResponse>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<Unit> GetAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<string> GetStringAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TDefaultError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<Stream> GetStreamAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<byte[]> GetByteArrayAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<TResponse> GetAsync<TResponse>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<Unit> GetAsync(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<string> GetStringAsync(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TDefaultError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<Stream> GetStreamAsync(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<byte[]> GetByteArrayAsync(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<TResponse> DeleteAsync<TResponse>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, templateUri, null, headers, false, cancellationToken);
        }

        public Task<Unit> DeleteAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, templateUri, null, headers, false, cancellationToken);
        }

        public Task<TResponse> DeleteAsync<TResponse>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<Unit> DeleteAsync(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<TResponse> PostAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<Unit> PostAsync(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<TResponse> PutAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Put, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<Unit> PutAsync(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Put, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<TResponse> PatchAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Patch, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<Unit> PatchAsync(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<TResponse> GetAsync<TResponse>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<Unit> GetAsync(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<string> GetStringAsync(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TDefaultError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<Stream> GetStreamAsync(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<byte[]> GetByteArrayAsync(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<TResponse> GetAsync<TResponse>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<Unit> GetAsync(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<string> GetStringAsync(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<string, TDefaultError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<Stream> GetStreamAsync(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<byte[]> GetByteArrayAsync(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<TResponse> DeleteAsync<TResponse>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, templateUri, null, null, false, cancellationToken);
        }

        public Task<Unit> DeleteAsync(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, templateUri, null, null, false, cancellationToken);
        }

        public Task<TResponse> DeleteAsync<TResponse>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<Unit> DeleteAsync(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<TResponse> PostAsync<TResponse>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<Unit> PostAsync(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<TResponse> PutAsync<TResponse>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Put, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<Unit> PutAsync(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Put, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<TResponse> PatchAsync<TResponse>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<TResponse, TDefaultError>(HttpMethod.Patch, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<Unit> PatchAsync(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryGetWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryGetWithCustomErrorAsync<TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(string, TError)> TryGetStringWithCustomErrorAsync<TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(Stream, TError)> TryGetStreamWithCustomErrorAsync<TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(byte[], TError)> TryGetByteArrayWithCustomErrorAsync<TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryGetWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(string, TError)> TryGetStringWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(Stream, TError)> TryGetStreamWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(byte[], TError)> TryGetByteArrayWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryDeleteWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Delete, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryDeleteWithCustomErrorAsync<TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Delete, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryDeleteWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Delete, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryDeleteWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Delete, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryPostWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryPostWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryPutWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Put, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryPutWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Put, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryPatchWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Patch, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryPatchWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryGetWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryGetWithCustomErrorAsync<TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<(string, TError)> TryGetStringWithCustomErrorAsync<TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<(Stream, TError)> TryGetStreamWithCustomErrorAsync<TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<(byte[], TError)> TryGetByteArrayWithCustomErrorAsync<TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryGetWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryGetWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(string, TError)> TryGetStringWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(Stream, TError)> TryGetStreamWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(byte[], TError)> TryGetByteArrayWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryDeleteWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Delete, templateUri, null, null, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryDeleteWithCustomErrorAsync<TError>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Delete, templateUri, null, null, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryDeleteWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Delete, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryDeleteWithCustomErrorAsync<TError>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Delete, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryPostWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryPostWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryPutWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Put, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryPutWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Put, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(TResponse, TError)> TryPatchWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TError>(HttpMethod.Patch, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(Unit, TError)> TryPatchWithCustomErrorAsync<TError>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryGetAsync<TResponse>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryGetAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(string, TDefaultError)> TryGetStringAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TDefaultError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(Stream, TDefaultError)> TryGetStreamAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(byte[], TDefaultError)> TryGetByteArrayAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryGetAsync<TResponse>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryGetAsync(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(string, TDefaultError)> TryGetStringAsync(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TDefaultError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(Stream, TDefaultError)> TryGetStreamAsync(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(byte[], TDefaultError)> TryGetByteArrayAsync(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryDeleteAsync<TResponse>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryDeleteAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, templateUri, null, headers, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryDeleteAsync<TResponse>(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryDeleteAsync(TemplateUri templateUri, object queryString, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, templateUri, queryString, headers, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryPostAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryPostAsync(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryPutAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Put, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryPutAsync(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Put, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryPatchAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Patch, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryPatchAsync(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, templateUri, requestData, headers, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryGetAsync<TResponse>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryGetAsync(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<(string, TDefaultError)> TryGetStringAsync(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TDefaultError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<(Stream, TDefaultError)> TryGetStreamAsync(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<(byte[], TDefaultError)> TryGetByteArrayAsync(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, templateUri, null, null, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryGetAsync<TResponse>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryGetAsync(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(string, TDefaultError)> TryGetStringAsync(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<string, TDefaultError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(Stream, TDefaultError)> TryGetStreamAsync(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Stream, TDefaultError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(byte[], TDefaultError)> TryGetByteArrayAsync(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<byte[], TDefaultError>(HttpMethod.Get, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryDeleteAsync<TResponse>(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, templateUri, null, null, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryDeleteAsync(TemplateUri templateUri, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, templateUri, null, null, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryDeleteAsync<TResponse>(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Delete, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryDeleteAsync(TemplateUri templateUri, object queryString, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Delete, templateUri, queryString, null, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryPostAsync<TResponse>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryPostAsync(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryPutAsync<TResponse>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Put, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryPutAsync(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Put, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(TResponse, TDefaultError)> TryPatchAsync<TResponse>(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<TResponse, TDefaultError>(HttpMethod.Patch, templateUri, requestData, null, false, cancellationToken);
        }

        public Task<(Unit, TDefaultError)> TryPatchAsync(TemplateUri templateUri, object requestData, CancellationToken cancellationToken)
        {
            return this.TrySendRequestAsync<Unit, TDefaultError>(HttpMethod.Post, templateUri, requestData, null, false, cancellationToken);
        }

        /// <summary>
        /// Получает токен доступа, который отправляется с Bearer.
        /// </summary>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(null);
        }

        protected virtual bool CheckResponseAsError(object response)
        {
            return false;
        }

        private static string ExtractMediaTypeFromHeaders(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                return null;
            }

            return headers.Remove("Content-Type", out var mediaType)
                ? mediaType
                : null;
        }

        private async Task<TResponse> SendWithHandlingExceptionsAsync<TResponse, TError>(
            HttpMethod httpMethod,
            TemplateUri templateUri,
            object requestData,
            IDictionary<string, string> headers,
            bool isAuthenticationRequest,
            CancellationToken cancellationToken)
        {
            var metricWriter = new RequestMetricWriter(
                this.metricsService,
                this.options?.Value?.StatusCodeMetricType,
                requestData,
                templateUri.GetTemplateUri());

            var logsCollector = this.CreateRequestLogsCollector();
            logsCollector.AddPath(templateUri.GetTemplateUri());

            var timeMetric = metricWriter.GetTimeMetric();
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                try
                {
                    var requestUri = this.GetRequestUrl<TResponse, TError>(templateUri);
                    if (requestData is not null && (httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Delete))
                    {
                        logsCollector.AddLogsFromPayload(requestData);
                        requestUri += requestUri.IndexOf("?", StringComparison.Ordinal) > 0 ? "&" : "?";
                        requestUri += await QueryStringSerializerHelper.SerializeObjectToQueryString(requestData, this.SerializeAdditionalConverters, this.IsCamelCase);
                    }

                    using var request = new HttpRequestMessage(httpMethod, requestUri);

                    var mediaType = ExtractMediaTypeFromHeaders(headers);
                    await this.AddHeadersAsync(request, headers, isAuthenticationRequest, cancellationToken);
                    if (requestData is not null && (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put || httpMethod == HttpMethod.Patch))
                    {
                        var httpContent = await this.CreateHttpContent(requestData, mediaType ?? this.MediaType, logsCollector);
                        request.Content = httpContent;
                    }

                    logsCollector.AddRequestHeaders(request.Headers.ToDictionary(x => x.Key, y => string.Join(';', y.Value)));

                    using var httpClient = await this.CreateHttpClientAsync<TError>();
                    if (this.RequestTimeout > 0)
                    {
                        httpClient.Timeout = TimeSpan.FromMilliseconds(this.RequestTimeout);
                    }

                    using var response = await httpClient.SendAsync(request, cancellationToken);

                    logsCollector.AddResponseHeaders(response.Headers.ToDictionary(x => x.Key, y => string.Join(';', y.Value)));
                    var deserializedResponse = await this.ReadResponseAsync<TResponse, TError>(response, logsCollector);
                    if (this.CheckResponseAsError(deserializedResponse))
                    {
                        await metricWriter.WriteMetricsAsError200(HttpStatusCode.OK).ConfigureAwait(false);
                    }
                    else
                    {
                        await metricWriter.WriteMetrics(response.StatusCode).ConfigureAwait(false);
                    }

                    return deserializedResponse;
                }
                catch (TaskCanceledException e) when (this.RequestTimeout > 0)
                {
                    throw new RequestException<TError>(e.Message, e, HttpStatusCode.GatewayTimeout);
                }
                catch (HttpRequestException e) when (e.InnerException is SocketException { SocketErrorCode: SocketError.TimedOut })
                {
                    throw new RequestException<TError>(e.Message, e, HttpStatusCode.GatewayTimeout);
                }
                catch (HttpRequestException e) when (e.InnerException is SocketException se)
                {
                    throw new RequestException<TError>(e.Message, e, HttpStatusCode.BadGateway);
                }
                catch (DeserializeException e)
                {
                    throw new RequestException<TError>(
                        $"{e.Message}. InnerResponse: {e.ResponseAsString}",
                        e,
                        (int)e.StatusCode < 400 ? HttpStatusCode.BadRequest : e.StatusCode);
                }
                catch (RequestException<TError>)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new RequestException<TError>("Failed request.", e, HttpStatusCode.InternalServerError);
                }
            }
            catch (RequestException<TError> e)
            {
                await metricWriter.WriteMetrics(e.StatusCode ?? HttpStatusCode.InternalServerError)
                    .ConfigureAwait(false);
                logsCollector.AddStatus((int)e.StatusCode);
                throw;
            }
            finally
            {
                if (timeMetric != null)
                {
                    await timeMetric.DisposeAsync().ConfigureAwait(false);
                }

                sw.Stop();
                logsCollector.AddRequestDuration(sw.Elapsed.TotalMilliseconds);
                logsCollector.WriteLogs();
            }
        }

        private async Task<HttpClient> CreateHttpClientAsync<TError>()
        {
            if (this.httpClientFactory != null)
            {
                return this.httpClientFactory.Create();
            }

            if (!this.NeedDownloadCertificate)
            {
                return new HttpClient();
            }

            if (string.IsNullOrEmpty(this.BaseUrl))
            {
                throw new RequestException<TError>("Failed download certificate. You must specify BaseUrl.", HttpStatusCode.BadRequest);
            }

            var domain = new Uri(this.BaseUrl).Host;
            var certificate = await this.DownloadCertificateAsync(domain);
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12
            };
            handler.ClientCertificates.Add(certificate);
            return new HttpClient(handler);
        }

        private async Task<X509Certificate2> DownloadCertificateAsync(string domain)
        {
            using var client = new TcpClient(domain, 443);
            await using var sslStream = new SslStream(client.GetStream(), true, (_, _, _, _) => true);
            await sslStream.AuthenticateAsClientAsync(domain);
            return new X509Certificate2(sslStream.RemoteCertificate!);
        }

        private void SetBaseUrl(IConfiguration configuration)
        {
            this.baseUrl = this.options?.Value?.BaseUrl;
            if (configuration == null)
            {
                return;
            }

            if (this.baseUrl == null)
            {
                return;
            }

            if (!this.baseUrl.StartsWith("${ConnectionStrings."))
            {
                return;
            }

            this.baseUrl = this.baseUrl.Replace("${ConnectionStrings.", string.Empty);
            this.baseUrl = this.baseUrl.TrimEnd('}');
            this.baseUrl = configuration.GetConnectionString(this.baseUrl);
        }

        private string GetRequestUrl<TResponse, TError>(TemplateUri templateUri)
        {
            var requestUri = templateUri.GetUri();
            requestUri = requestUri?.IndexOf("http://") == 0 || requestUri?.IndexOf("https://") == 0
                ? requestUri
                : $"{this.BaseUrl}{requestUri}";

            var message = $"Failed url: {requestUri}.";
            if (requestUri.IndexOf("http") == -1 && string.IsNullOrEmpty(this.BaseUrl))
            {
                message += " Base URL not specified.";
            }

            return !Helper.CheckUrlIsValid(requestUri)
                ? throw new RequestException<TError>(message, HttpStatusCode.BadRequest)
                : requestUri;
        }

        private async Task<HttpContent> CreateHttpContent(object requestData, string mediaType, RequestLogsCollector logsCollector)
        {
            if (requestData is byte[] requestDataAsBytes)
            {
                return this.CreateHttpContent(requestDataAsBytes);
            }

            if (requestData is MultipartFormData multipartFormData)
            {
                return this.CreateMultipartFormDataContent(multipartFormData);
            }

            logsCollector.AddLogsFromPayload(requestData);
            var serializedRequestData = await this.SerializeRequestData(requestData, mediaType);

            if (this.LogRequest)
            {
                logsCollector.AddRequest(requestData);
            }

            return new StringContent(serializedRequestData, Encoding.UTF8, mediaType);
        }

        private HttpContent CreateMultipartFormDataContent(MultipartFormData multipartFormData)
        {
            var content = new MultipartFormDataContent(this.DefaultBoundary);
            foreach (var item in multipartFormData)
            {
                var boundaryContent = item.Value is byte[] bytes
                    ? new ByteArrayContent(bytes)
                    : (HttpContent)new StringContent(
                        JsonSerializerHelper.SerializeObjectToJson(item.Value, this.SerializeAdditionalConverters, this.IsCamelCase),
                        item.Encoding,
                        item.MediaType);
                boundaryContent.Headers.Add("Content-Type", item.MediaType);
                boundaryContent.Headers.Add("Content-Disposition", $"form-data; name=\"{item.Key}\"; filename=\"{item.FileName}\"");
                content.Add(boundaryContent);
            }

            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", $"multipart/form-data; boundary={this.DefaultBoundary}");
            return content;
        }

        private async Task<string> SerializeRequestData(object requestData, string mediaType)
        {
            var serializedRequestData = mediaType switch
            {
                "application/x-www-form-urlencoded" => await QueryStringSerializerHelper.SerializeObjectToQueryString(requestData, this.SerializeAdditionalConverters, this.IsCamelCase),
                "text/xml" => XmlSerializerHelper.SerializeObjectToXml(requestData),
                _ => JsonSerializerHelper.SerializeObjectToJson(requestData, this.SerializeAdditionalConverters, this.IsCamelCase)
            };
            return serializedRequestData;
        }

        private HttpContent CreateHttpContent(byte[] requestData)
        {
            var content = new ByteArrayContent(requestData);
            return content;
        }

        private async Task<TResponse> ReadResponseAsync<TResponse, TError>(HttpResponseMessage response, RequestLogsCollector logsCollector)
        {
            return !response.IsSuccessStatusCode
                ? throw new RequestException<TError>("Failed response.", await this.ReadResponseAsync<TError>(response, logsCollector), response.StatusCode, await response.Content.ReadAsStringAsync())
                : await this.ReadResponseAsync<TResponse>(response, logsCollector);
        }

        private async Task<TResponse> ReadResponseAsync<TResponse>(HttpResponseMessage response, RequestLogsCollector logsCollector)
        {
            logsCollector.AddStatus(response.IsSuccessStatusCode ? 200 : (int)response.StatusCode);

            if (typeof(TResponse) == typeof(Unit))
            {
                return default;
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }

            if (typeof(TResponse) == typeof(Stream))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                return (TResponse)((object)stream);
            }

            if (typeof(TResponse) == typeof(byte[]))
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                return (TResponse)((object)bytes);
            }

            var responseAsString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseAsString))
            {
                return default;
            }

            var deserializedResponse = this.DeserializeResponse<TResponse>(response, responseAsString, logsCollector);
            if (deserializedResponse is IHasStatusCode responseWithStatusCode)
            {
                responseWithStatusCode.StatusCode = (int)response.StatusCode;
            }

            return deserializedResponse;
        }

        private TResponse DeserializeResponse<TResponse>(HttpResponseMessage response, string responseAsString, RequestLogsCollector logsCollector)
        {
            try
            {
                var mediaType = response.Content?.Headers?.ContentType?.MediaType ?? this.MediaType;
                var isXml = mediaType.Equals("text/xml", StringComparison.OrdinalIgnoreCase);
                var responseAsObj = isXml
                    ? XmlSerializerHelper.DeserializeObjectFromXml<TResponse>(responseAsString)
                    : JsonSerializerHelper.DeserializeObjectFromJson<TResponse>(responseAsString, this.DeserializeAdditionalConverters, this.AdditionalScalarStructureTypes);

                logsCollector.AddResponse(responseAsObj);
                logsCollector.AddLogsFromPayload(responseAsObj);
                return responseAsObj;
            }
            catch (Exception e)
            {
                logsCollector.AddResponse(responseAsString);
                throw new DeserializeException(e, responseAsString, response.StatusCode);
            }
        }

        private async Task AddHeadersAsync(HttpRequestMessage request, IDictionary<string, string> headers, bool isAuthenticationRequest, CancellationToken cancellationToken)
        {
            if (!isAuthenticationRequest)
            {
                await this.AddBearerAuthorization(request, cancellationToken).ConfigureAwait(false);
            }

            if (headers != null)
            {
                foreach (var (key, value) in headers)
                {
                    request.Headers.Add(key, value);
                }
            }
        }

        private async Task AddBearerAuthorization(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(this.AccessToken))
            {
                this.AccessToken = await AsyncAwaiter
                    .AwaitResultAsync("get_access_token_key", () => this.GetAccessTokenAsync(cancellationToken))
                    .ConfigureAwait(false);
            }

            var scheme = string.IsNullOrEmpty(this.options?.Value?.AuthenticationHeaderScheme)
                             ? "Bearer"
                             : this.options.Value.AuthenticationHeaderScheme;
            if (!string.IsNullOrEmpty(this.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(scheme, this.AccessToken);
            }
        }

        private async Task<TResponse> DurableRequestAsync<TResponse, TError>(Func<Task<TResponse>> func, CancellationToken cancellationToken)
        {
            short currentAttempts = 1;

            short authenticateCurrentAttempts = 1;
            const short authenticateMaxAttempts = 3;

            while (true)
            {
                try
                {
                    return await func().ConfigureAwait(false);
                }
                catch (RequestException<TError> e) when ((int)e.StatusCode == 401 || (int)e.StatusCode == 403)
                {
                    if (authenticateCurrentAttempts > authenticateMaxAttempts)
                    {
                        this.logger?.LogDebug("Limit of authorize attempts is exceeded");
                        throw;
                    }

                    if (string.IsNullOrEmpty(this.AccessToken))
                    {
                        throw;
                    }

                    this.AccessToken = null;
                    authenticateCurrentAttempts++;
                }
                catch (RequestException<TError> e) when ((int)e.StatusCode >= this.DurableFromHttpStatus)
                {
                    this.logger?.LogError(e, "Error");

                    if (currentAttempts > this.Attempts)
                    {
                        this.logger?.LogDebug("Limit of attempts is exceeded");
                        throw;
                    }

                    currentAttempts++;

                    await Task.Delay(this.MillisecondsDelay, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private RequestLogsCollector CreateRequestLogsCollector()
        {
            var logLevelsByHttpStatus = new List<List<LogLevelOptions>>
            {
                this.LogLevelsByHttpStatus,
                this.options?.Value?.LogLevelsByHttpStatus,
                this.requestLogsCollectorOptions?.Value?.LogLevelsByHttpStatus,
                RequestLogsCollectorOptions.DefaultLogLevelsByHttpStatus
            };

            if (this.logsCollectorFactory != null)
            {
                return this.logsCollectorFactory.Create(this.logger, logLevelsByHttpStatus);
            }

            return new RequestLogsCollector(this.logger, logLevelsByHttpStatus);
        }
    }
}
