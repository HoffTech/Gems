// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Gems.Logging.Mvc.LogsCollector;
using Gems.Metrics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

namespace Gems.Http.Tests
{
    public class DefaultClientServiceTests
    {
        [Test]
        public async Task TestTrySendRequestAsync()
        {
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(x => x.Create())
                .Returns(new HttpClient());

            var requestLogCollectorMock = new Mock<IRequestLogsCollectorFactory>();
            requestLogCollectorMock
                .Setup(x => x.Create(It.IsAny<ILogger>(), null))
                .Returns(new RequestLogsCollector(NullLogger.Instance, null));

            var client = new DefaultClientService(
                Mock.Of<IOptions<HttpClientServiceOptions>>(),
                new BaseClientServiceHelper(
                    Mock.Of<IMetricsService>(),
                    Mock.Of<ILogger<BaseClientServiceHelper>>(),
                    requestLogCollectorMock.Object,
                    null,
                    httpClientFactoryMock.Object,
                    null));

            var (response, error) = await client.TrySendRequestAsync<string, string>(
                HttpMethod.Get,
                "https://ya.ru".ToTemplateUri(),
                new DeviceInfoRequest() { Limit = 10, Offset = 10 },
                null,
                CancellationToken.None);

            Assert.IsNull(error);
            Assert.IsNotEmpty(response);
        }
    }
}
