// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Jobs.Quartz.Configuration;

public class SqlDataSourceOptions
{
    public string Provider { get; set; }

    public string ConnectionString { get; set; }

    public string ConnectionStringName { get; set; }

    public string ConnectionProviderType { get; set; }
}
