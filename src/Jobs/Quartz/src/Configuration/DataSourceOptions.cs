// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Jobs.Quartz.Configuration;

public class DataSourceOptions
{
    public SqlDataSourceOptions SqlServer { get; set; }

    public SqlDataSourceOptions OracleOdp { get; set; }

    public SqlDataSourceOptions OracleOdpManaged { get; set; }

    public SqlDataSourceOptions MySql { get; set; }

    public SqlDataSourceOptions Sqlite { get; set; }

    public SqlDataSourceOptions SqliteMicrosoft { get; set; }

    public SqlDataSourceOptions Firebird { get; set; }

    public SqlDataSourceOptions Npgsql { get; set; }
}
