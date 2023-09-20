// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;

namespace Gems.Data.UnitOfWork
{
    public class DefaultConnectionStringProvider : IConnectionStringProvider
    {
        public const string DefaultConnectionName = "DefaultConnection";

        public DefaultConnectionStringProvider(string value)
        {
            this.Value = value;
        }

        public DefaultConnectionStringProvider(string name, IConfiguration config)
        {
            this.Value = config.GetConnectionString(name);
        }

        public DefaultConnectionStringProvider(IConfiguration config) : this(DefaultConnectionName, config)
        {
        }

        public string Value { get; }
    }
}
