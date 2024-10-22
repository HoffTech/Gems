// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Utils
{
    internal class AsyncDisposableContainer : IDisposable, IAsyncDisposable
    {
        private readonly Stack<object> components = new Stack<object>();
        private bool disposed;
        private bool disposedAsync;

        public void RegisterComponent(object o)
        {
            this.components.Push(o);
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (this.disposed)
            {
                return;
            }

            if (this.disposedAsync)
            {
                return;
            }

            while (this.components.Count > 0)
            {
                var component = this.components.Pop();
                if (component is IAsyncDisposable asyncDisposableComponent)
                {
                    await asyncDisposableComponent.DisposeAsync();
                }
                else if (component is IDisposable disposableComponent)
                {
                    disposableComponent.Dispose();
                }
            }

            this.disposedAsync = true;
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (!this.disposedAsync)
                {
                    this.DisposeAsync()
                        .AsTask()
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }

            this.disposed = true;
        }
    }
}
