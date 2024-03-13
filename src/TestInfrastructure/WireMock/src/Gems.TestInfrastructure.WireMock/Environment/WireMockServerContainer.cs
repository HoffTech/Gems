using FluentAssertions;

using WireMock.Server;
using WireMock.Settings;

namespace Gems.TestInfrastructure.WireMock.Environment
{
    public class WireMockServerContainer : IDisposable
    {
        private bool disposedValue;

        public WireMockServerContainer(WireMockServerSettings settings)
        {
            this.Settings = settings;
        }

        public WireMockServerSettings Settings { get; set; } = null;

        public WireMockServer WireMockServer { get; set; } = null;

        public void Start()
        {
            if (this.WireMockServer == null)
            {
                this.Settings.Should().NotBeNull();
                this.WireMockServer = WireMockServer.Start(this.Settings);
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (this.WireMockServer != null)
                    {
                        if (this.WireMockServer.IsStarted)
                        {
                            this.WireMockServer.Stop();
                        }

                        this.WireMockServer.Dispose();
                    }
                }

                this.disposedValue = true;
            }
        }
    }
}
